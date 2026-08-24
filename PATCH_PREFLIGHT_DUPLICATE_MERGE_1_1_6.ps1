$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
$backup = Join-Path $root 'ProgramV110.cs.1.1.0.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
$text = Get-Content $source -Raw -Encoding UTF8

# Faqat yakuniy candidate filterini qo'llaymiz; topish mexanizmi o'zgarmaydi.
$oldFilter = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
if ($text.Contains($oldFilter)) {
    $text = $text.Replace($oldFilter, '.Where(IsRelevantCandidate)')
}

if (-not $text.Contains('private static bool IsRelevantCandidate(AddinCandidate x)')) {
$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }
$method = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);
            if (IsKnownNonTranslatorProduct(productText)) return false;

            var explicitTranslator = FunctionWords.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var strongPair = StrongFunctionPairs.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeHost = string.Equals(x.Host, "Word", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.Host, "Excel", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.Host, "Office", StringComparison.OrdinalIgnoreCase);
            if (officeHost) return strongPair || explicitTranslator;
            return strongPair || explicitTranslator || x.Score >= 70;
        }

        private static bool IsKnownNonTranslatorProduct(string productText)
        {
            if (string.IsNullOrWhiteSpace(productText)) return false;
            var known = new[] {
                "igor pavlov", "7 zip", "7zip", "winrar", "google chrome", "chrome",
                "telegram desktop", "telegram", "github desktop", "git", "visual studio code",
                "vscode", "visual studio", "e imzo", "uzcrypto", "zoom workplace", "zoom",
                "easeus data recovery", "minitool power data recovery", "workflow manager",
                "microsoft access", "microsoft excel", "microsoft word", "microsoft outlook",
                "microsoft powerpoint", "microsoft publisher", "microsoft project", "microsoft office",
                "microsoft proofing", "microsoft language pack", "microsoft visual studio tools"
            };
            return known.Any(p => productText.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

'@
$text = $text.Replace($marker, $method + $marker)
}

# Eng muhim tuzatish: aynan bir xil canonical mahsulot nomi turli publisher bilan
# kelganida (TransLit/Translit) publisher identity'ni bo'lib yubormaydi.
$pattern = '(?s)        private static string BuildProductIdentity\(AddinCandidate item\)\s*\{.*?\n        \}\s*\n\s*        private static AddinCandidate MergeCandidate'
$replacement = @'
        private static string BuildProductIdentity(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";
            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;

            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);

            // Cross-source merge: Word Add-in va Windows Uninstall bir xil
            // mahsulotni turli publisher/host ma'lumotlari bilan berishi mumkin.
            if (IsMergeableFunctionalProduct(product))
                return "FUNC:" + product;

            return "APP:" + product + "|" + publisher;
        }

        private static bool IsMergeableFunctionalProduct(string product)
        {
            if (string.IsNullOrWhiteSpace(product)) return false;
            var names = new[] { "translit", "translit gt", "translitgt", "translator", "transliterator", "translation" };
            return names.Contains(product, StringComparer.OrdinalIgnoreCase);
        }

        private static AddinCandidate MergeCandidate'@
$newText = [regex]::Replace($text, $pattern, $replacement, 1)
if ($newText -eq $text) { throw 'BuildProductIdentity qismi topilmadi yoki almashtirilmadi.' }
Set-Content -Path $source -Value $newText -Encoding UTF8
Write-Host '1.1.6 duplicate merge patch qo`llandi.'

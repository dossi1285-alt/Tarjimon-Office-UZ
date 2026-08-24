$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
$backup = Join-Path $root 'ProgramV110.cs.1.1.0.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
$text = Get-Content $source -Raw -Encoding UTF8

# 1) 1.1.4 da ishlagan Office/non-translator filtrini stabil 1.1.0 source ustiga qayta qo'llaymiz.
$oldFilter = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
if (-not $text.Contains($oldFilter)) { throw 'Stabil 1.1.0 filter qatori topilmadi.' }
$text = $text.Replace($oldFilter, '.Where(IsRelevantCandidate)')

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }

$filterMethod = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);

            if (IsKnownNonTranslatorProduct(productText)) return false;

            var explicitProductTranslator = FunctionWords.Any(w =>
                productText.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var explicitTranslator = FunctionWords.Any(w =>
                text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var strongPair = StrongFunctionPairs.Any(w =>
                text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeSignal = text.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("word", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("excel", StringComparison.OrdinalIgnoreCase) >= 0;

            if (string.Equals(x.Host, "Word", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Excel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Office", StringComparison.OrdinalIgnoreCase))
                return strongPair || explicitProductTranslator || (explicitTranslator && officeSignal);

            return strongPair || explicitProductTranslator || (explicitTranslator && officeSignal) || x.Score >= 70;
        }

        private static bool IsKnownNonTranslatorProduct(string productText)
        {
            if (string.IsNullOrWhiteSpace(productText)) return false;

            var knownProducts = new[]
            {
                "igor pavlov", "7-zip", "7zip", "winrar",
                "google chrome", "chrome", "telegram desktop", "telegram",
                "github desktop", "git", "visual studio code", "vscode",
                "visual studio", "e-imzo", "uzcrypto", "zoom workplace",
                "easeus data recovery", "minitool power data recovery",
                "workflow manager",
                "microsoft access", "microsoft excel", "microsoft word",
                "microsoft outlook", "microsoft powerpoint", "microsoft publisher",
                "microsoft project", "microsoft office", "microsoft proofing",
                "microsoft language pack", "microsoft visual studio tools"
            };

            return knownProducts.Any(p =>
                productText.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

'@
$text = $text.Replace($marker, $filterMethod + $marker)

# 2) Bir xil mahsulotning turli aniqlash manbalarini jamlash.
# Hozirgi muammo: "TransLit" (Word, publisher noma'lum) va
# "Translit" (Windows, Microsoft) nomlari alohida publisher sababli ikki qator bo'lib chiqmoqda.
$oldIdentity = @'
        private static string BuildProductIdentity(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";
            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;
            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);
            return "APP:" + product + "|" + publisher;
        }
'@
if (-not $text.Contains($oldIdentity)) { throw 'BuildProductIdentity stabil qismi topilmadi.' }

$newIdentity = @'
        private static string BuildProductIdentity(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";
            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;

            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);

            // Bir mahsulot turli manbalarda turlicha ko'rinishi mumkin:
            // Word Add-in registryda publisher noma'lum, Windows Uninstallda esa Microsoft.
            // Translit shu holatning amaldagi test namunasi.
            // Faqat oldindan ma'lum bo'lgan umumiy translator nomlari uchun
            // publisher farqini e'tiborsiz qoldiramiz; boshqa mahsulotlar xavfsiz
            // tarzda Product + Publisher bo'yicha alohida qoladi.
            if (IsCrossSourceFunctionalIdentity(product))
                return "FUNC:" + product;

            return "APP:" + product + "|" + publisher;
        }

        private static bool IsCrossSourceFunctionalIdentity(string product)
        {
            if (string.IsNullOrWhiteSpace(product)) return false;

            var knownSharedNames = new[]
            {
                "translit",
                "translit gt",
                "translitgt"
            };

            return knownSharedNames.Contains(product, StringComparer.OrdinalIgnoreCase);
        }
'@
$text = $text.Replace($oldIdentity, $newIdentity)

Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host '1.1.5 duplicate merge patch qo`llandi.'

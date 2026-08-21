$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }

$backup = Join-Path $root 'ProgramV110.cs.1.1.2.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
$text = Get-Content $source -Raw -Encoding UTF8

# Current stable 1.1.x source uses the original final candidate filter.
$oldFilter = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
$newFilter = '.Where(IsRelevantCandidate)'
if (-not $text.Contains($oldFilter)) {
    if ($text.Contains($newFilter) -and $text.Contains('IsKnownNonTranslatorProduct')) {
        Write-Host '1.1.3 known-program filter allaqachon qo`llangan.'
        exit 0
    }
    throw 'Kutilgan stabil candidate filter topilmadi.'
}
$text = $text.Replace($oldFilter, $newFilter)

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
$method = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            // Office registry/startup nomzodlari funksional qidiruvdan o'tgan.
            if (string.Equals(x.Host, "Word", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Excel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Office", StringComparison.OrdinalIgnoreCase))
                return x.Score >= 35;

            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);

            // Ma'lum, lekin tarjimon bo'lmagan Windows dasturlarini filtrlash.
            if (IsKnownNonTranslatorProduct(productText)) return false;

            var explicitProductTranslator = FunctionWords.Any(w => productText.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var explicitTranslator = FunctionWords.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var strongPair = StrongFunctionPairs.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeSignal = text.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("word", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("excel", StringComparison.OrdinalIgnoreCase) >= 0;

            return strongPair || explicitProductTranslator || (explicitTranslator && officeSignal) || x.Score >= 70;
        }

        private static bool IsKnownNonTranslatorProduct(string productText)
        {
            if (string.IsNullOrWhiteSpace(productText)) return false;

            // Igor Pavlov = 7-Zip. Bu tarjimon/add-in emas.
            var knownProducts = new[]
            {
                "igor pavlov", "7-zip", "7zip", "winrar", "google chrome", "chrome",
                "telegram desktop", "telegram", "github desktop", "visual studio code",
                "vscode", "visual studio", "e-imzo", "uzcrypto", "zoom workplace",
                "easeus data recovery", "minitool power data recovery", "workflow manager"
            };

            if (knownProducts.Any(p => productText.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;

            // Microsoft Office komponentlari, Proofing va Runtime tarjimon emas.
            if (productText.IndexOf("microsoft", StringComparison.OrdinalIgnoreCase) >= 0 &&
                (productText.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("proofing", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("language pack", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("runtime", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("visual studio tools", StringComparison.OrdinalIgnoreCase) >= 0))
                return true;

            return false;
        }

'@
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }
$text = $text.Replace($marker, $method + $marker)
Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host '1.1.3 known-program filter patch qo`llandi.'

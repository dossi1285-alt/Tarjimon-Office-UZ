$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }

$backup = Join-Path $root 'ProgramV110.cs.1.1.2.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
$text = Get-Content $source -Raw -Encoding UTF8

$old = @'
            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);
            var explicitProductTranslator = FunctionWords.Any(w => productText.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var explicitTranslator = FunctionWords.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var strongPair = StrongFunctionPairs.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeSignal = text.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("word", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("excel", StringComparison.OrdinalIgnoreCase) >= 0;

            return strongPair || explicitProductTranslator || (explicitTranslator && officeSignal) || x.Score >= 70;
'@
$new = @'
            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);

            // Known non-translator products: they may contain generic Office/Windows
            // metadata but are not Office translation/conversion add-ins.
            if (IsKnownNonTranslatorProduct(productText)) return false;

            var explicitProductTranslator = FunctionWords.Any(w => productText.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var explicitTranslator = FunctionWords.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var strongPair = StrongFunctionPairs.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeSignal = text.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("word", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("excel", StringComparison.OrdinalIgnoreCase) >= 0;

            return strongPair || explicitProductTranslator || (explicitTranslator && officeSignal) || x.Score >= 70;
'@
if (-not $text.Contains($old)) { throw 'IsRelevantCandidate filtri topilmadi.' }
$text = $text.Replace($old, $new)

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
$method = @'
        private static bool IsKnownNonTranslatorProduct(string productText)
        {
            if (string.IsNullOrWhiteSpace(productText)) return false;

            // Igor Pavlov = 7-Zip. Bu tarjimon/add-in emas.
            var knownProducts = new[]
            {
                "igor pavlov", "7-zip", "7zip", "winrar", "winrar", "rar",
                "google chrome", "chrome", "telegram desktop", "telegram",
                "git", "github desktop", "visual studio code", "vscode",
                "visual studio", "e-imzo", "uzcrypto", "zoom workplace",
                "easeus data recovery", "minitool power data recovery",
                "workflow manager", "microsoft office", "microsoft access",
                "microsoft excel", "microsoft word", "microsoft outlook",
                "microsoft powerpoint", "microsoft publisher", "microsoft project",
                "microsoft proofing", "microsoft language pack", "microsoft visual studio tools"
            };

            if (knownProducts.Any(p => productText.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0))
                return true;

            // Microsoft komponentlari: faqat mahsulotning o'zida tarjima funksiyasi
            // ko'rsatilmagan bo'lsa, umumiy Office komponentlarini chiqaramiz.
            if (productText.IndexOf("microsoft", StringComparison.OrdinalIgnoreCase) >= 0 &&
                (productText.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("visual studio", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("proofing", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("language pack", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 productText.IndexOf("runtime", StringComparison.OrdinalIgnoreCase) >= 0))
                return true;

            return false;
        }

'@
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }
$text = $text.Replace($marker, $method + $marker)
Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host '1.1.3 known-program filter patch qo`llandi.'

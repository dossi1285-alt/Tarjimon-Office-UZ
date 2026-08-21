$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
$backup = Join-Path $root 'ProgramV110.cs.1.1.0.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
$text = Get-Content $source -Raw -Encoding UTF8

$oldFilter = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
if (-not $text.Contains($oldFilter)) { throw 'Stabil 1.1.0 filter qatori topilmadi.' }
$text = $text.Replace($oldFilter, '.Where(IsRelevantCandidate)')

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }

$method = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);

            // 1) Avval ma'lum oddiy mahsulotlarni filtrlash.
            // Bu tekshiruv Office host tekshiruvidan OLDIN bajariladi.
            // Shuning uchun Microsoft Office MUI/Proofing/Runtime komponentlari
            // score 35 bo'lsa ham tarjimon sifatida chiqmaydi.
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
$text = $text.Replace($marker, $method + $marker)
Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host '1.1.4 Office component filter patch qo`llandi.'

$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }

$text = Get-Content $source -Raw -Encoding UTF8
$old = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
$new = '.Where(IsRelevantCandidate)'

if ($text.Contains($new)) {
    Write-Host '1.1.2 filter patch allaqachon qo`llangan.'
    exit 0
}
if (-not $text.Contains($old)) { throw 'Kutilgan ScanCandidates filter qatori topilmadi.' }

$text = $text.Replace($old, $new)
$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }

$method = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var allText = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);
            var functionSignal = FunctionWords.Any(w => allText.Contains(w));
            var strongSignal = StrongFunctionPairs.Any(w => allText.Contains(w));
            var microsoft = (x.Publisher ?? string.Empty).IndexOf("microsoft", StringComparison.OrdinalIgnoreCase) >= 0;

            // Microsoft Office/MUI/Proofing/Runtime komponentlari tarjimon emas.
            // Faqat ularda haqiqiy tarjima/transliteratsiya signali bo'lsa qoldiramiz.
            if (microsoft && !functionSignal && !strongSignal &&
                (productText.Contains("microsoft office") ||
                 productText.Contains("office ") ||
                 productText.Contains(" office") ||
                 productText.Contains("office") ||
                 productText.Contains("proofing") ||
                 productText.Contains("microsoft visual studio tools for office") ||
                 productText.Contains("office runtime") ||
                 productText.Contains("microsoft access") ||
                 productText.Contains("microsoft excel") ||
                 productText.Contains("microsoft word") ||
                 productText.Contains("microsoft outlook") ||
                 productText.Contains("microsoft powerpoint") ||
                 productText.Contains("microsoft publisher") ||
                 productText.Contains("microsoft onenote") ||
                 productText.Contains("microsoft skype")))
                return false;

            // Office Add-in/Startup nomzodlari funksional qidiruvdan o'tgan bo'ladi.
            if (string.Equals(x.Host, "Word", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Excel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Office", StringComparison.OrdinalIgnoreCase))
                return functionSignal || strongSignal || x.Score >= 45;

            // Windows Uninstall ro'yxatida faqat haqiqiy tarjima signali bo'lgan mahsulotlar.
            return functionSignal || strongSignal || x.Score >= 70;
        }

'@
$text = $text.Replace($marker, $method + $marker)
Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host 'Preflight 1.1.2 filter patch qo`llandi.'

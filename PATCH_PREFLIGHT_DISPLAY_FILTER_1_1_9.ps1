$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
$backup = Join-Path $root 'ProgramV110.cs.1.1.9.filter-backup'

Write-Host '==============================================='
Write-Host 'Tarjimon Office UZ - 1.1.9 Display Filter patch'
Write-Host '==============================================='

if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force; Write-Host 'Backup yaratildi.' }

$text = Get-Content $source -Raw -Encoding UTF8
$old = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
$new = '.Where(IsDisplayRelevantCandidate)'

if ($text.Contains($new)) {
    Write-Host '1.1.9 display filter allaqachon qo`llangan.'
    exit 0
}
if (-not $text.Contains($old)) { throw 'Asosiy ScanCandidates filter qismi topilmadi. Kod o`zgartirilmadi.' }
$text = $text.Replace($old, $new)

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi. Kod o`zgartirilmadi.' }

$method = @'
        // Faqat jadvalga chiqarish filtri.
        // Qidiruv manbalari, semantic scan va duplicate merge o'zgarmaydi.
        private static bool IsDisplayRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var product = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation, x.Registration);
            var publisher = NormalizeSearchText(x.Publisher);
            var host = NormalizeSearchText(x.Host);

            var strongPair = StrongFunctionPairs.Any(product.Contains);
            var hasTranslatorSignal =
                product.Contains("translit") ||
                product.Contains("transliteration") ||
                product.Contains("transliterator") ||
                product.Contains("translator") ||
                product.Contains("translation") ||
                product.Contains("tarjimon") ||
                product.Contains("переводчик") ||
                product.Contains("перевод") ||
                product.Contains("preslov") ||
                product.Contains("preslovlj") ||
                product.Contains("savodxon");

            var hasLanguagePair =
                (product.Contains("kirill") && product.Contains("lotin")) ||
                (product.Contains("cyrillic") && product.Contains("latin")) ||
                (product.Contains("kiril") && product.Contains("latin"));

            var isOfficeCandidate =
                host.Contains("word") || host.Contains("excel") || host.Contains("office") ||
                product.Contains("\\addins\\") || product.Contains("startup") || product.Contains("xlstart");

            // Microsoft Office MUI/Proofing/Shared komponentlari tarjimon emas.
            // Kuchli translator signali bo'lmasa jadvalga chiqmaydi.
            if (publisher.Contains("microsoft") && !strongPair && !hasTranslatorSignal) return false;

            // Office/AppData add-inlar: funksional signal bo'lsa noma'lum mahsulot ham qoladi.
            if (isOfficeCandidate && (strongPair || hasTranslatorSignal || hasLanguagePair)) return true;

            // Windows Installed Programs: generic converter/convert signalining o'zi yetarli emas.
            // Bu Igor Pavlov/7-Zip kabi false positive'larni kesadi.
            if (strongPair || hasLanguagePair) return true;
            if (hasTranslatorSignal && x.Score >= 35) return true;

            return false;
        }

'@
$text = $text.Replace($marker, $method + $marker)
Set-Content -Path $source -Value $text -Encoding UTF8

Write-Host 'OK - 1.1.9 Display Filter patch qo`llandi.'
Write-Host 'Qidiruv va duplicate merge kodiga tegilmadi.'
Write-Host "Source: $source"
Write-Host '==============================================='

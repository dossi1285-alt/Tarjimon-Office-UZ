$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
$backup = Join-Path $root 'ProgramV110.cs.1.1.0.backup'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }

# Backupni obj ichiga emas, repository rootiga qo'yamiz: build script obj papkasini tozalasa ham backup saqlanadi.
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }

$text = Get-Content $source -Raw -Encoding UTF8
$oldFilter = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
$newFilter = '.Where(IsRelevantCandidate)'
if (-not $text.Contains($newFilter)) {
    if (-not $text.Contains($oldFilter)) { throw 'Kutilgan ScanCandidates filter qatori topilmadi.' }
    $text = $text.Replace($oldFilter, $newFilter)
}

$oldScore = 'Score = Math.Min(Math.Max(score, own ? 90 : 35), 100),'
$newScore = 'Score = own ? 90 : Math.Min(score + (officeAssociation ? 20 : 0), 100),'
if ($text.Contains($oldScore)) {
    $text = $text.Replace($oldScore, $newScore)
}

if (-not $text.Contains('private static bool IsRelevantCandidate(AddinCandidate x)')) {
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

            // Windows Uninstall ro'yxatidan oddiy dasturlarni tarjimon sifatida ko'rsatmaslik.
            // Tarjimonning mahsulot nomi yoki metadata'sida aniq signal bo'lsa saqlaymiz.
            var productText = NormalizeSearchText(x.Product, x.Publisher);
            var text = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);
            var explicitProductTranslator = FunctionWords.Any(w => productText.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var explicitTranslator = FunctionWords.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var strongPair = StrongFunctionPairs.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeSignal = text.IndexOf("office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("word", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               text.IndexOf("excel", StringComparison.OrdinalIgnoreCase) >= 0;

            return strongPair || explicitProductTranslator || (explicitTranslator && officeSignal) || x.Score >= 70;
        }

'@
    if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }
    $text = $text.Replace($marker, $method + $marker)
}

Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host 'Preflight 1.1.1 filter patch qo`llandi.'

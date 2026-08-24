$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
$backup = "$source.1.1.10.backup"

Write-Host '==============================================='
Write-Host 'Tarjimon Office UZ - 1.1.10 Strict Display Filter'
Write-Host '==============================================='

if (!(Test-Path $source)) { throw "Source topilmadi: $source" }
Copy-Item $source $backup -Force
Write-Host "Backup yaratildi: $backup"

$text = [IO.File]::ReadAllText($source, [Text.Encoding]::UTF8)

# Current source may already contain the intermediate IsDisplayRelevantCandidate filter.
$hasCurrentFilter = $text.Contains('.Where(IsDisplayRelevantCandidate)')
$hasOldFilter = [regex]::IsMatch($text, '(?m)^\s*\.Where\(x\s*=>\s*x\.IsOwnProduct\s*\|\|\s*x\.Score\s*>=\s*35\)')

if ($hasOldFilter) {
    $text = [regex]::Replace($text, '(?m)^\s*\.Where\(x\s*=>\s*x\.IsOwnProduct\s*\|\|\s*x\.Score\s*>=\s*35\)', '                .Where(IsDisplayRelevantCandidate)', 1)
    Write-Host 'Eski display filter qatori yangilandi.'
}
elseif ($hasCurrentFilter) {
    Write-Host 'Mavjud IsDisplayRelevantCandidate filter topildi.'
}
else {
    Write-Host 'Source dagi Where qatorlari:'
    [regex]::Matches($text, '(?m)^\s*\.Where\([^\r\n]+\)') | ForEach-Object { Write-Host $_.Value }
    throw 'Display filter qatori topilmadi. Bu source boshqa filter versiyasiga ega.'
}

# Replace the existing relevance method if present; otherwise insert it before ScanOfficeAddins.
$methodPattern = '(?s)\s*private\s+static\s+bool\s+IsDisplayRelevantCandidate\s*\(\s*AddinCandidate\s+x\s*\)\s*\{.*?\n\s*\}\s*(?=private\s+static\s+void\s+ScanOfficeAddins)'
$method = @'

        private static bool IsDisplayRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var text = NormalizeSearchText(
                x.Product ?? string.Empty,
                x.Publisher ?? string.Empty,
                x.Host ?? string.Empty,
                x.Evidence ?? string.Empty,
                x.Registration ?? string.Empty,
                x.InstallLocation ?? string.Empty,
                x.StartupFile ?? string.Empty);

            string[] noise =
            {
                "microsoft office mui", "microsoft office proofing", "proofing tools",
                "office shared", "office 32-bit components", "office professional plus",
                "language pack", "языковой пакет", "microsoft visual studio tools",
                "visual studio tools", "microsoft silverlight"
            };
            foreach (var n in noise)
                if (text.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0) return false;

            string[] strong =
            {
                "translit", "transliteration", "transliterator", "translator", "translation",
                "tarjimon", "savodxon", "переводчик", "перевод", "preslov", "preslovljav",
                "kirill-lotin", "lotin-kirill", "kirill to lotin", "lotin to kirill",
                "cyrillic latin", "latin cyrillic", "cyrillic to latin", "latin to cyrillic"
            };

            if (strong.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0)) return true;

            var hasKirill = text.IndexOf("kirill", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            text.IndexOf("cyrillic", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            text.IndexOf("кирил", StringComparison.OrdinalIgnoreCase) >= 0;
            var hasLatin = text.IndexOf("lotin", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           text.IndexOf("latin", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           text.IndexOf("латин", StringComparison.OrdinalIgnoreCase) >= 0;
            if (hasKirill && hasLatin) return true;

            return false;
        }
'@

if ([regex]::IsMatch($text, $methodPattern)) {
    $text = [regex]::Replace($text, $methodPattern, $method, 1)
    Write-Host 'Mavjud IsDisplayRelevantCandidate metodi yangilandi.'
}
else {
    $marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
    if (!$text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi. Source o`zgargan.' }
    $text = $text.Replace($marker, $method + "`r`n" + $marker)
    Write-Host 'IsDisplayRelevantCandidate metodi qo`shildi.'
}

[IO.File]::WriteAllText($source, $text, [Text.Encoding]::UTF8)
Write-Host 'OK - 1.1.10 Strict Display Filter qo`llandi.'
Write-Host 'Qidiruv, duplicate merge va uninstall kodiga tegilmadi.'
Write-Host "Source: $source"
Write-Host '==============================================='

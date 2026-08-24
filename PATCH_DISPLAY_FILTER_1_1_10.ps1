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

if ($text.Contains('private static bool IsDisplayTranslatorCandidate(AddinCandidate x)')) {
    Write-Host '1.1.10 display filter allaqachon mavjud. Patch qayta qo`llanmaydi.'
    exit 0
}

$pattern = '(?m)^(\s*)\.Where\s*\(\s*x\s*=>\s*x\.IsOwnProduct\s*\|\|\s*x\.Score\s*>=\s*35\s*\)'
$match = [regex]::Match($text, $pattern)
if (!$match.Success) {
    $pattern = '\.Where\s*\(\s*x\s*=>\s*x\.IsOwnProduct\s*\|\|\s*x\.Score\s*>=\s*35\s*\)'
    $match = [regex]::Match($text, $pattern)
}
if (!$match.Success) {
    Write-Host 'Source dagi Where qatorlari:'
    $text -split "`r?`n" | Where-Object { $_ -match '\.Where' } | ForEach-Object { Write-Host $_ }
    throw 'Display filter qatori topilmadi. Bu source boshqa filter versiyasiga ega.'
}

$replacement = [regex]::Replace($match.Value, '\.Where\s*\(\s*x\s*=>\s*x\.IsOwnProduct\s*\|\|\s*x\.Score\s*>=\s*35\s*\)', '.Where(IsDisplayTranslatorCandidate)')
$text = $text.Remove($match.Index, $match.Length).Insert($match.Index, $replacement)

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (!$text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi. Source o`zgargan; patch bekor qilindi.' }

$method = @'
        private static bool IsDisplayTranslatorCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;
            var text = NormalizeSearchText(x.Product ?? string.Empty, x.Publisher ?? string.Empty,
                x.Host ?? string.Empty, x.Evidence ?? string.Empty, x.Registration ?? string.Empty,
                x.InstallLocation ?? string.Empty, x.StartupFile ?? string.Empty);

            string[] noise = { "microsoft office mui", "microsoft office proofing", "proofing tools",
                "office shared", "office 32-bit components", "office professional plus", "language pack",
                "языковой пакет", "microsoft visual studio tools", "visual studio tools", "microsoft silverlight" };
            foreach (var n in noise) if (text.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0) return false;

            string[] strong = { "translit", "transliteration", "transliterator", "translator", "translation",
                "tarjimon", "savodxon", "переводчик", "перевод", "preslov", "preslovljav",
                "kirill-lotin", "lotin-kirill", "kirill to lotin", "lotin to kirill",
                "cyrillic latin", "latin cyrillic", "cyrillic to latin", "latin to cyrillic" };
            if (strong.Any(w => text.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0)) return true;

            var hasKirill = text.IndexOf("kirill", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            text.IndexOf("cyrillic", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            text.IndexOf("кирил", StringComparison.OrdinalIgnoreCase) >= 0;
            var hasLatin = text.IndexOf("lotin", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           text.IndexOf("latin", StringComparison.OrdinalIgnoreCase) >= 0 ||
                           text.IndexOf("латин", StringComparison.OrdinalIgnoreCase) >= 0;
            return hasKirill && hasLatin;
        }

'@
$text = $text.Replace($marker, $method + $marker)
[IO.File]::WriteAllText($source, $text, [Text.Encoding]::UTF8)
Write-Host 'OK - 1.1.10 Strict Display Filter qo`llandi.'
Write-Host 'Qidiruv, duplicate merge va uninstall kodiga tegilmadi.'
Write-Host "Source: $source"
Write-Host '==============================================='

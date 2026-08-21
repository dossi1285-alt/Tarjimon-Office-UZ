$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
$backupDir = Join-Path $root 'obj\PreflightPatchBackup'
New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
$backup = Join-Path $backupDir 'ProgramV110.cs'
Copy-Item $source $backup -Force
$text = Get-Content $source -Raw -Encoding UTF8
$old = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
$new = '.Where(IsRelevantCandidate)'
if ($text.Contains($new)) { Write-Host 'Filter patch allaqachon qo`llangan.'; exit 0 }
if (-not $text.Contains($old)) { throw 'Kutilgan ScanCandidates filter qatori topilmadi.' }
$text = $text.Replace($old, $new)
$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
$method = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            // Office registry/startup nomzodlari allaqachon funksional qidiruvdan o'tgan.
            if (string.Equals(x.Host, "Word", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Excel", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Host, "Office", StringComparison.OrdinalIgnoreCase))
                return x.Score >= 35;

            // Windows Uninstall ro'yxatidan oddiy dasturlarni Office tarjimoni deb ko'rsatmaslik.
            // Mahsulot nomining o'zida aniq tarjima/transliteratsiya signali bo'lsa uni saqlaymiz.
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
Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host 'Preflight 1.1.1 filter patch qo`llandi.'

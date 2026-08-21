$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$program = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\Program.cs'
$package = Join-Path $root 'TarjimonOfficeUZ.Setup.Wix\Package.wxs'

$p = Get-Content -Raw -LiteralPath $program

$old0 = @'
            first.IsOwnProduct = group.Any(x => x.IsOwnProduct);
'@
$new0 = @'
            first.IsOwnProduct = group.Any(x => x.IsOwnProduct);
            if (first.IsOwnProduct)
            {
                first.Product = "Tarjimon Office UZ";
                first.Publisher = "Dostonjon Ashurov";
            }
'@
if (-not $p.Contains($old0)) { throw 'Program.cs: own-product normalization point not found.' }
$p = $p.Replace($old0, $new0)

$old1 = @'
                var candidates = ScanCandidates();
                if (candidates.Count > 0)
'@
$new1 = @'
                // Safety: verify the new MSI before any existing product can be removed.
                var msi = ExtractMsi();
                var candidates = ScanCandidates();
                if (candidates.Count > 0)
'@
if (-not $p.Contains($old1)) { throw 'Program.cs: Main safety insertion point not found.' }
$p = $p.Replace($old1, $new1)

$old2 = @'
                var msi = ExtractMsi();
                var result = Process.Start(new ProcessStartInfo
'@
$new2 = @'
                var result = Process.Start(new ProcessStartInfo
'@
if (-not $p.Contains($old2)) { throw 'Program.cs: duplicate MSI extraction point not found.' }
$p = $p.Replace($old2, $new2)

$old3 = @'
        private static string ExtractMsi()
        {
            var resource = typeof(Program).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("TarjimonOfficeUZSetup.msi", StringComparison.OrdinalIgnoreCase));
            if (resource == null) throw new FileNotFoundException("Embedded MSI topilmadi.");
            var path = Path.Combine(Path.GetTempPath(), "TarjimonOfficeUZ", Guid.NewGuid().ToString("N") + ".msi");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var input = typeof(Program).Assembly.GetManifestResourceStream(resource))
            using (var output = File.Create(path)) input.CopyTo(output);
            return path;
        }
'@
$new3 = @'
        private static string ExtractMsi()
        {
            var resource = typeof(Program).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("TarjimonOfficeUZSetup.msi", StringComparison.OrdinalIgnoreCase));
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (resource == null)
            {
                var sidecar = new[]
                {
                    Path.Combine(baseDir, "TarjimonOfficeUZSetup.msi"),
                    Path.Combine(baseDir, "TarjimonOfficeUZ.Setup.msi")
                }.FirstOrDefault(File.Exists);

                if (!string.IsNullOrWhiteSpace(sidecar)) return sidecar;
                throw new FileNotFoundException("Tarjimon Office UZ MSI topilmadi. O‘rnatish boshlanishidan oldin MSI mavjud bo‘lishi shart.");
            }

            var path = Path.Combine(Path.GetTempPath(), "TarjimonOfficeUZ", Guid.NewGuid().ToString("N") + ".msi");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var input = typeof(Program).Assembly.GetManifestResourceStream(resource))
            using (var output = File.Create(path)) input.CopyTo(output);
            return path;
        }
'@
if (-not $p.Contains($old3)) { throw 'Program.cs: ExtractMsi block not found.' }
$p = $p.Replace($old3, $new3)
[System.IO.File]::WriteAllText($program, $p, (New-Object System.Text.UTF8Encoding($false)))

$w = Get-Content -Raw -LiteralPath $package
$w2 = $w.Replace('Manufacturer="Tarjimon Office UZ"', 'Manufacturer="Dostonjon Ashurov"')
if ($w2 -eq $w) { throw 'Package.wxs: Manufacturer field not found.' }
[System.IO.File]::WriteAllText($package, $w2, (New-Object System.Text.UTF8Encoding($false)))

Write-Host 'PATCH OK'
Write-Host 'Own product normalized to Tarjimon Office UZ / Dostonjon Ashurov.'
Write-Host 'MSI now verified before uninstall.'
Write-Host 'Sidecar MSI fallback added.'
Write-Host 'MSI Manufacturer set to Dostonjon Ashurov.'

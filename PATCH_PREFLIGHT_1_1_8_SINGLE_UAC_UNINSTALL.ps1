$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
$backup = Join-Path $root 'ProgramV110.cs.1.1.0.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
if (-not (Test-Path $backup)) { throw 'Stabil backup yaratilmadi.' }
$text = Get-Content $source -Raw -Encoding UTF8

$mainPattern = '(?s)        \[STAThread\]\s+private static int Main\(\)\s*\{.*?\n        \}\s*\n\s*        private static List<AddinCandidate> ScanCandidates'
$newMain = @'
        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args.Length >= 3 && string.Equals(args[1], "--elevated-uninstall", StringComparison.OrdinalIgnoreCase))
                    return RunElevatedWorkflow(args[2]);

                var candidates = ScanCandidates();
                using (var form = new ReviewForm(candidates))
                {
                    if (form.ShowDialog() != DialogResult.OK) return 1602;
                    var selected = form.SelectedItems.ToList();
                    if (selected.Count == 0) return StartMsiInstall();

                    if (IsAdministrator())
                        return RunSelectedAndInstall(selected);

                    var selectionFile = Path.Combine(Path.GetTempPath(), "TarjimonOfficeUZ_Selected_" + Guid.NewGuid().ToString("N") + ".txt");
                    File.WriteAllLines(selectionFile,
                        selected.Select(x => Convert.ToBase64String(Encoding.UTF8.GetBytes(x.Registration ?? x.Product ?? string.Empty))),
                        Encoding.UTF8);

                    var exe = Application.ExecutablePath;
                    using (var elevated = Process.Start(new ProcessStartInfo
                    {
                        FileName = exe,
                        Arguments = "--elevated-uninstall \"" + selectionFile + "\"",
                        UseShellExecute = true,
                        Verb = "runas"
                    }))
                    {
                        if (elevated == null) return 1603;
                        elevated.WaitForExit();
                        return elevated.ExitCode;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Tarjimon Office UZ — Installer xatosi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1603;
            }
        }

        private static bool IsAdministrator()
        {
            try
            {
                using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
                {
                    var principal = new System.Security.Principal.WindowsPrincipal(identity);
                    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
            }
            catch { return false; }
        }

        private static int RunElevatedWorkflow(string selectionFile)
        {
            try
            {
                if (!IsAdministrator()) return 1603;
                if (!File.Exists(selectionFile)) return 1603;

                var registrations = new HashSet<string>(
                    File.ReadAllLines(selectionFile, Encoding.UTF8)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => Encoding.UTF8.GetString(Convert.FromBase64String(x))),
                    StringComparer.OrdinalIgnoreCase);

                var candidates = ScanCandidates();
                var selected = candidates
                    .Where(x => registrations.Contains(x.Registration) || registrations.Contains(x.Product ?? string.Empty))
                    .ToList();

                foreach (var item in selected)
                {
                    if (!TryUninstall(item))
                    {
                        MessageBox.Show("'" + item.Product + "' uchun o'chirish amalga oshmadi. Boshqa mahsulotlarga tegilmadi.",
                            "Tarjimon Office UZ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return 1603;
                    }
                }

                try { File.Delete(selectionFile); } catch { }
                return StartMsiInstall();
            }
            catch (Exception ex)
            {
                try { if (File.Exists(selectionFile)) File.Delete(selectionFile); } catch { }
                MessageBox.Show(ex.ToString(), "Tarjimon Office UZ — Installer xatosi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1603;
            }
        }

        private static int RunSelectedAndInstall(List<AddinCandidate> selected)
        {
            foreach (var item in selected)
            {
                if (!TryUninstall(item))
                {
                    MessageBox.Show("'" + item.Product + "' uchun o'chirish amalga oshmadi. Boshqa mahsulotlarga tegilmadi.",
                        "Tarjimon Office UZ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return 1603;
                }
            }
            return StartMsiInstall();
        }

        private static int StartMsiInstall()
        {
            var msi = ExtractMsi();
            var result = Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = "/i \"" + msi + "\" /passive",
                UseShellExecute = true
            });
            return result == null ? 1603 : 0;
        }

        private static List<AddinCandidate> ScanCandidates
'@
if (-not [regex]::IsMatch($text, $mainPattern)) { throw 'Main qismi topilmadi.' }
$text = [regex]::Replace($text, $mainPattern, $newMain, 1)

$runPattern = '(?s)        private static bool RunUninstall\(string commandLine\)\s*\{.*?\n        \}\s*\n\s*        private static string\[\] ExtractPaths'
$newRun = @'
        private static bool RunUninstall(string commandLine)
        {
            string fileName, arguments;
            var trimmed = commandLine.TrimStart();
            if (trimmed.StartsWith("msiexec", StringComparison.OrdinalIgnoreCase))
            {
                fileName = "msiexec.exe";
                var space = trimmed.IndexOf(' ');
                arguments = space >= 0 ? trimmed.Substring(space + 1).Trim() : string.Empty;
                arguments = Regex.Replace(arguments, @"(?i)(^|\s)/(i|x)(?=\s*\{)", "$1/X");
                if (!Regex.IsMatch(arguments, @"(?i)(^|\s)/x(?=\s*\{")) return false;
            }
            else if (trimmed.StartsWith("\""))
            {
                var end = trimmed.IndexOf('"', 1);
                fileName = end > 0 ? trimmed.Substring(1, end - 1) : trimmed.Trim('"');
                arguments = end > 0 ? trimmed.Substring(end + 1).Trim() : string.Empty;
            }
            else
            {
                var split = trimmed.IndexOf(' ');
                fileName = split > 0 ? trimmed.Substring(0, split) : trimmed;
                arguments = split > 0 ? trimmed.Substring(split + 1) : string.Empty;
            }
            try
            {
                using (var p = Process.Start(new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = true
                }))
                {
                    if (p == null) return false;
                    p.WaitForExit();
                    return p.ExitCode == 0 || p.ExitCode == 3010 || p.ExitCode == 1641;
                }
            }
            catch { return false; }
        }

        private static string[] ExtractPaths
'@
if (-not [regex]::IsMatch($text, $runPattern)) { throw 'RunUninstall qismi topilmadi.' }
$text = [regex]::Replace($text, $runPattern, $newRun, 1)
Set-Content -Path $source -Value $text -Encoding UTF8
Write-Host '1.1.8 single-UAC uninstall patch qo`llandi.'

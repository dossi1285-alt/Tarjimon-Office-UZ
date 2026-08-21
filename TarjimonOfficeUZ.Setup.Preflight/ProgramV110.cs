using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TarjimonOfficeUZ.Setup.Preflight
{
    internal sealed class AddinCandidate
    {
        public string Product { get; set; }
        public string Publisher { get; set; }
        public string Version { get; set; }
        public string Host { get; set; }
        public string Registration { get; set; }
        public string UninstallString { get; set; }
        public string InstallLocation { get; set; }
        public string StartupFile { get; set; }
        public bool IsOwnProduct { get; set; }
        public int Score { get; set; }
        public string Evidence { get; set; }
    }

    internal static class ProgramV110
    {
        private static readonly string[] FunctionWords =
        {
            "translit", "transliteration", "transliterator", "translator", "translation",
            "tarjimon", "переводчик", "перевод", "preslov", "preslovljav", "preslovljanje",
            "kirill", "kiril", "cyrillic", "кирилл", "lotin", "latin", "латин",
            "uzbek", "o'zbek", "узбек", "konvert", "converter", "conversion", "convert",
            "kirill-lotin", "lotin-kirill", "kirill to lotin", "lotin to kirill"
        };

        private static readonly string[] StrongFunctionPairs =
        {
            "kirill lotin", "lotin kirill", "cyrillic latin", "latin cyrillic",
            "cyrillic to latin", "latin to cyrillic", "kirill to latin", "latin to kirill",
            "kirill lotin converter", "lotin kirill converter", "translit word", "translit office",
            "word translit", "office translit", "preslovljanje", "preslovljavanje",
            "kirill - lotin", "lotin - kirill", "kirill → lotin", "lotin → kirill"
        };

        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var candidates = ScanCandidates();
                using (var form = new ReviewForm(candidates))
                {
                    if (form.ShowDialog() != DialogResult.OK) return 1602;
                    foreach (var item in form.SelectedItems)
                    {
                        if (!TryUninstall(item))
                        {
                            MessageBox.Show("'" + item.Product + "' uchun o'chirish amalga oshmadi. Boshqa mahsulotlarga tegilmadi.",
                                "Tarjimon Office UZ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return 1603;
                        }
                    }
                }

                var msi = ExtractMsi();
                var result = Process.Start(new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = "/i \"" + msi + "\" /passive",
                    UseShellExecute = true,
                    Verb = "runas"
                });
                return result == null ? 1603 : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Tarjimon Office UZ — Installer xatosi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1603;
            }
        }

        private static List<AddinCandidate> ScanCandidates()
        {
            var list = new List<AddinCandidate>();
            var views = Environment.Is64BitOperatingSystem
                ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                : new[] { RegistryView.Default };

            ScanOfficeAddins(list, views);
            ScanStartupLocations(list);
            ScanInstalledPrograms(list, views);

            return list
                .GroupBy(BuildProductIdentity, StringComparer.OrdinalIgnoreCase)
                .Select(MergeCandidate)
                .Where(x => x.IsOwnProduct || x.Score >= 35)
                .OrderByDescending(x => x.IsOwnProduct)
                .ThenByDescending(x => x.Score)
                .ThenBy(x => x.Product)
                .ToList();
        }

        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)
        {
            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            foreach (var host in new[] { "Word", "Excel" })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            using (var addins = root.OpenSubKey("SOFTWARE\\Microsoft\\Office\\" + host + "\\Addins"))
            {
                if (addins == null) continue;
                foreach (var keyName in addins.GetSubKeyNames())
                using (var key = addins.OpenSubKey(keyName))
                {
                    if (key == null) continue;
                    var friendly = Convert.ToString(key.GetValue("FriendlyName")) ?? keyName;
                    var description = Convert.ToString(key.GetValue("Description")) ?? string.Empty;
                    var manifest = Convert.ToString(key.GetValue("Manifest")) ?? string.Empty;
                    var progId = Convert.ToString(key.GetValue("ProgId")) ?? string.Empty;
                    var assembly = Convert.ToString(key.GetValue("Assembly")) ?? string.Empty;
                    var text = NormalizeSearchText(keyName, friendly, description, manifest, progId, assembly);
                    var paths = ExtractPaths(manifest, assembly);
                    foreach (var path in paths) text += " " + ReadSemanticFileText(path);

                    var score = ScoreSemantic(text);
                    if (score <= 0) continue;
                    score += 35;
                    if (!string.IsNullOrWhiteSpace(manifest) || !string.IsNullOrWhiteSpace(assembly)) score += 5;

                    var publisher = string.Empty;
                    var version = string.Empty;
                    var uninstall = string.Empty;
                    FindUninstall(friendly, keyName, out publisher, out version, out uninstall);
                    list.Add(new AddinCandidate
                    {
                        Product = friendly,
                        Publisher = publisher,
                        Version = version,
                        Host = host,
                        Registration = hive + "\\" + view + "\\Office\\" + host + "\\Addins\\" + keyName,
                        UninstallString = uninstall,
                        InstallLocation = FirstExistingDirectory(paths),
                        IsOwnProduct = IsOwnProduct(text, publisher),
                        Score = Math.Min(score, 100),
                        Evidence = "Office " + host + " Addins metadata; " + BuildSemanticEvidence(text)
                    });
                }
            }
        }

        private static void ScanInstalledPrograms(List<AddinCandidate> list, RegistryView[] views)
        {
            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            using (var parent = root.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
            {
                if (parent == null) continue;
                foreach (var name in parent.GetSubKeyNames())
                using (var key = parent.OpenSubKey(name))
                {
                    if (key == null) continue;
                    var display = Convert.ToString(key.GetValue("DisplayName")) ?? string.Empty;
                    var publisher = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                    var version = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty;
                    var uninstall = Convert.ToString(key.GetValue("QuietUninstallString")) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(uninstall)) uninstall = Convert.ToString(key.GetValue("UninstallString")) ?? string.Empty;
                    var installLocation = Convert.ToString(key.GetValue("InstallLocation")) ?? string.Empty;
                    var displayIcon = Convert.ToString(key.GetValue("DisplayIcon")) ?? string.Empty;
                    var url = Convert.ToString(key.GetValue("URLInfoAbout")) ?? string.Empty;
                    var text = NormalizeSearchText(name, display, publisher, installLocation, displayIcon, url);
                    if (Directory.Exists(installLocation)) text += " " + ReadDirectorySemanticText(installLocation);

                    var own = IsOwnProduct(text, publisher);
                    var score = ScoreSemantic(text);
                    var officeAssociation = ContainsOfficeAssociation(installLocation, displayIcon, url);
                    if (!own && score < 15 && !officeAssociation) continue;
                    if (!own && score < 25 && !officeAssociation) continue;

                    score += officeAssociation ? 20 : 0;
                    list.Add(new AddinCandidate
                    {
                        Product = display,
                        Publisher = publisher,
                        Version = version,
                        Host = officeAssociation ? "Office" : "Windows",
                        Registration = hive + "\\" + view + "\\Uninstall\\" + name,
                        UninstallString = uninstall,
                        InstallLocation = installLocation,
                        IsOwnProduct = own,
                        Score = Math.Min(Math.Max(score, own ? 90 : 35), 100),
                        Evidence = "Windows Uninstall registry; " + BuildSemanticEvidence(text)
                    });
                }
            }
        }

        private static void ScanStartupLocations(List<AddinCandidate> list)
        {
            var paths = new List<string>();
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!string.IsNullOrWhiteSpace(appData))
            {
                paths.Add(Path.Combine(appData, "Microsoft", "Word", "STARTUP"));
                paths.Add(Path.Combine(appData, "Microsoft", "Excel", "XLSTART"));
                paths.Add(Path.Combine(appData, "Microsoft", "Templates"));
                paths.Add(Path.Combine(appData, "Microsoft", "AddIns"));
                paths.Add(Path.Combine(appData, "Microsoft", "Office"));
            }
            if (!string.IsNullOrWhiteSpace(localAppData)) paths.Add(Path.Combine(localAppData, "Microsoft", "Office"));

            AddConfiguredStartupPaths(paths, "Word", "STARTUP-PATH");
            AddConfiguredStartupPaths(paths, "Excel", "OPEN");

            foreach (var directory in paths.Distinct(StringComparer.OrdinalIgnoreCase))
                ScanStartupDirectory(list, directory);
        }

        private static void ScanStartupDirectory(List<AddinCandidate> list, string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return;
            string[] files;
            try { files = Directory.GetFiles(directory); } catch { return; }
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                if (!new[] { ".dot", ".dotm", ".dotx", ".wll", ".xla", ".xlam", ".xll", ".vsto", ".dll", ".exe", ".xml" }.Contains(ext, StringComparer.OrdinalIgnoreCase)) continue;
                var text = NormalizeSearchText(Path.GetFileName(file), file, ReadSemanticFileText(file));
                var score = ScoreSemantic(text);
                if (score <= 0) continue;
                string publisher, version, uninstall;
                FindUninstall(Path.GetFileNameWithoutExtension(file), Path.GetFileName(file), out publisher, out version, out uninstall);
                list.Add(new AddinCandidate
                {
                    Product = Path.GetFileNameWithoutExtension(file),
                    Publisher = publisher,
                    Version = version,
                    Host = directory.IndexOf("XLSTART", StringComparison.OrdinalIgnoreCase) >= 0 ? "Excel" : "Word",
                    Registration = "Office/AppData: " + directory,
                    UninstallString = uninstall,
                    InstallLocation = Path.GetDirectoryName(file),
                    StartupFile = file,
                    IsOwnProduct = IsOwnProduct(text, publisher),
                    Score = Math.Min(score + 35, 100),
                    Evidence = "Office/AppData startup fayli; " + BuildSemanticEvidence(text)
                });
            }
        }

        private static bool TryUninstall(AddinCandidate item)
        {
            if (!string.IsNullOrWhiteSpace(item.UninstallString))
                return RunUninstall(item.UninstallString);

            var fallback = FindFallbackUninstaller(item.InstallLocation);
            if (!string.IsNullOrWhiteSpace(fallback)) return RunUninstall(fallback);

            // Registry-only Office registrations and startup files have no Windows uninstall command.
            // Remove only the selected translator registration/file, never unrelated Office data.
            var removed = RemoveOfficeRegistration(item.Registration);
            if (!removed && !string.IsNullOrWhiteSpace(item.StartupFile) && File.Exists(item.StartupFile))
            {
                try { File.Delete(item.StartupFile); removed = true; } catch { }
            }
            return removed;
        }

        private static string FindFallbackUninstaller(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return string.Empty;
            try
            {
                var names = new[] { "uninstall.exe", "unins000.exe", "unins001.exe", "setup.exe" };
                foreach (var name in names)
                {
                    var path = Path.Combine(directory, name);
                    if (File.Exists(path)) return "\"" + path + "\"";
                }
                foreach (var file in Directory.GetFiles(directory, "unins*.exe")) return "\"" + file + "\"";
            }
            catch { }
            return string.Empty;
        }

        private static bool RemoveOfficeRegistration(string registration)
        {
            if (string.IsNullOrWhiteSpace(registration) || !registration.Contains("Office\\")) return false;
            try
            {
                var m = Regex.Match(registration, @"^(CurrentUser|LocalMachine)\\(Registry64|Registry32|Default)\\Office\\(Word|Excel)\\Addins\\(.+)$", RegexOptions.IgnoreCase);
                if (!m.Success) return false;
                var hive = m.Groups[1].Value.Equals("CurrentUser", StringComparison.OrdinalIgnoreCase) ? RegistryHive.CurrentUser : RegistryHive.LocalMachine;
                var view = m.Groups[2].Value.Equals("Registry32", StringComparison.OrdinalIgnoreCase) ? RegistryView.Registry32 : RegistryView.Registry64;
                var host = m.Groups[3].Value;
                var addin = m.Groups[4].Value;
                using (var root = RegistryKey.OpenBaseKey(hive, view))
                using (var office = root.OpenSubKey("SOFTWARE\\Microsoft\\Office\\" + host + "\\Addins", true))
                {
                    if (office == null) return false;
                    office.DeleteSubKeyTree(addin, false);
                    return true;
                }
            }
            catch { return false; }
        }

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
                if (!Regex.IsMatch(arguments, @"(?i)(^|\s)/x(?=\s*\{)")) return false;
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
                using (var p = Process.Start(new ProcessStartInfo { FileName = fileName, Arguments = arguments, UseShellExecute = true, Verb = "runas" }))
                {
                    if (p == null) return false;
                    p.WaitForExit();
                    return p.ExitCode == 0 || p.ExitCode == 3010 || p.ExitCode == 1641;
                }
            }
            catch { return false; }
        }

        private static string[] ExtractPaths(string manifest, string assembly)
        {
            var result = new List<string>();
            foreach (var value in new[] { manifest, assembly })
            {
                if (string.IsNullOrWhiteSpace(value)) continue;
                var cleaned = value.Replace("file:///", "").Replace("file://", "");
                var pipe = cleaned.IndexOf('|');
                if (pipe >= 0) cleaned = cleaned.Substring(0, pipe);
                cleaned = Environment.ExpandEnvironmentVariables(cleaned);
                if (File.Exists(cleaned)) result.Add(cleaned);
            }
            return result.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private static string ReadSemanticFileText(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) return string.Empty;
            var text = new StringBuilder();
            try
            {
                var info = FileVersionInfo.GetVersionInfo(file);
                text.Append(' ').Append(info.ProductName).Append(' ').Append(info.CompanyName).Append(' ').Append(info.FileDescription).Append(' ').Append(info.InternalName).Append(' ').Append(info.OriginalFilename);
            }
            catch { }

            var ext = Path.GetExtension(file);
            try
            {
                if (new[] { ".dotx", ".dotm", ".xlam", ".xla" }.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    text.Append(' ').Append(ReadOfficePackageText(file));
                else if (new[] { ".xml", ".vsto" }.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    text.Append(' ').Append(File.ReadAllText(file, Encoding.UTF8).Substring(0, Math.Min(200000, (int)new FileInfo(file).Length)));
                else if (new[] { ".dll", ".exe", ".wll" }.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    text.Append(' ').Append(ReadBinaryStrings(file));
            }
            catch { }
            return text.ToString();
        }

        private static string ReadOfficePackageText(string file)
        {
            var sb = new StringBuilder();
            using (var zip = ZipFile.OpenRead(file))
            foreach (var entry in zip.Entries.Where(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).Take(80))
            {
                using (var reader = new StreamReader(entry.Open(), Encoding.UTF8, true))
                {
                    var s = reader.ReadToEnd();
                    sb.Append(' ').Append(s);
                }
                if (sb.Length > 300000) break;
            }
            return sb.ToString();
        }

        private static string ReadBinaryStrings(string file)
        {
            var data = File.ReadAllBytes(file);
            var limit = Math.Min(data.Length, 8 * 1024 * 1024);
            var sb = new StringBuilder();
            var ascii = new StringBuilder();
            var unicode = new StringBuilder();
            for (var i = 0; i < limit; i++)
            {
                var b = data[i];
                if (b >= 32 && b <= 126) ascii.Append((char)b); else { if (ascii.Length >= 4) sb.Append(' ').Append(ascii); ascii.Clear(); }
                if (i + 1 < limit && b >= 32 && b <= 126 && data[i + 1] == 0)
                {
                    unicode.Append((char)b); i++;
                }
                else if (unicode.Length >= 4) { sb.Append(' ').Append(unicode); unicode.Clear(); }
            }
            if (ascii.Length >= 4) sb.Append(' ').Append(ascii);
            if (unicode.Length >= 4) sb.Append(' ').Append(unicode);
            return sb.ToString();
        }

        private static string ReadDirectorySemanticText(string directory)
        {
            try
            {
                return string.Join(" ", Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)
                    .Where(f => new[] { ".dll", ".exe", ".vsto", ".dotm", ".xlam", ".xla", ".xml" }.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                    .Take(20).Select(ReadSemanticFileText));
            }
            catch { return string.Empty; }
        }

        private static int ScoreSemantic(string text)
        {
            var normalized = NormalizeSearchText(text);
            if (string.IsNullOrWhiteSpace(normalized)) return 0;
            var strong = StrongFunctionPairs.Count(normalized.Contains);
            var matches = FunctionWords.Where(normalized.Contains).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (strong > 0) return 45 + Math.Min(strong, 2) * 5;
            if (matches.Contains("translit") || matches.Contains("transliteration") || matches.Contains("transliterator")) return 35 + Math.Min(matches.Count - 1, 3) * 5;
            if (matches.Contains("translator") || matches.Contains("translation") || matches.Contains("tarjimon") || matches.Contains("переводчик") || matches.Contains("перевод")) return 35 + Math.Min(matches.Count - 1, 3) * 5;
            if (matches.Contains("kirill") && matches.Contains("lotin")) return 40;
            if (matches.Contains("cyrillic") && matches.Contains("latin")) return 40;
            if (matches.Count >= 3 && (matches.Contains("kirill") || matches.Contains("cyrillic") || matches.Contains("lotin") || matches.Contains("latin"))) return 30;
            if (matches.Count >= 3) return 25;
            return matches.Count == 2 ? 18 : 0;
        }

        private static string BuildSemanticEvidence(string text)
        {
            var normalized = NormalizeSearchText(text);
            var matches = FunctionWords.Where(normalized.Contains).Distinct(StringComparer.OrdinalIgnoreCase).Take(6).ToArray();
            return matches.Length == 0 ? "Funksional signal topilmadi" : "Funksional signal: " + string.Join(", ", matches);
        }

        private static string NormalizeSearchText(params string[] values)
        {
            return string.Join(" ", values.Where(x => !string.IsNullOrWhiteSpace(x))).ToLowerInvariant().Replace("-", " ").Replace("_", " ");
        }

        private static bool IsOwnProduct(string text, string publisher)
        {
            var p = (publisher ?? string.Empty).ToLowerInvariant();
            return text.Contains("tarjimon office uz") || text.Contains("tarjimonofficeuz") || p.Contains("tarjimon office uz") || p.Contains("tarjimonofficeuz");
        }

        private static bool ContainsOfficeAssociation(params string[] values)
        {
            var text = NormalizeSearchText(values);
            return text.Contains("microsoft office") || text.Contains("office\\") || text.Contains("\\office") || text.Contains("startup") || text.Contains("xlstart") || text.Contains("\\addins\\") || text.Contains("\\addin\\");
        }

        private static string FirstExistingDirectory(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir)) return dir;
            }
            return string.Empty;
        }

        private static string BuildProductIdentity(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";
            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;
            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);
            return "APP:" + product + "|" + publisher;
        }

        private static AddinCandidate MergeCandidate(IGrouping<string, AddinCandidate> group)
        {
            var first = group.OrderByDescending(x => x.Score).First();
            first.Score = group.Max(x => x.Score);
            first.IsOwnProduct = group.Any(x => x.IsOwnProduct);
            first.Host = string.Join(", ", group.Select(x => x.Host).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x));
            first.UninstallString = group.Select(x => x.UninstallString).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.UninstallString;
            first.InstallLocation = group.Select(x => x.InstallLocation).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.InstallLocation;
            first.StartupFile = group.Select(x => x.StartupFile).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.StartupFile;
            first.Publisher = group.Select(x => x.Publisher).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.Publisher;
            first.Version = group.Select(x => x.Version).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.Version;
            first.Evidence = string.Join("; ", group.Select(x => x.Evidence).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Take(3));
            return first;
        }

        private static string ExtractProductCode(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine)) return string.Empty;
            var m = Regex.Match(commandLine, @"\{[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}\}");
            return m.Success ? m.Value.ToUpperInvariant() : string.Empty;
        }

        private static string NormalizeIdentity(string value)
        {
            var normalized = Regex.Replace((value ?? string.Empty).Trim().ToLowerInvariant(), @"\s+", " ");
            return Regex.Replace(normalized.Replace("_", " ").Replace("-", " "), @"[\\./]+", " ").Trim();
        }

        private static void FindUninstall(string friendly, string keyName, out string publisher, out string version, out string uninstall)
        {
            publisher = string.Empty; version = string.Empty; uninstall = string.Empty;
            var views = Environment.Is64BitOperatingSystem ? new[] { RegistryView.Registry64, RegistryView.Registry32 } : new[] { RegistryView.Default };
            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            using (var parent = root.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
            {
                if (parent == null) continue;
                foreach (var name in parent.GetSubKeyNames())
                using (var key = parent.OpenSubKey(name))
                {
                    if (key == null) continue;
                    var display = Convert.ToString(key.GetValue("DisplayName")) ?? string.Empty;
                    var candidate = NormalizeSearchText(display, name);
                    var target = NormalizeSearchText(friendly, keyName);
                    if (string.IsNullOrWhiteSpace(display) || (!candidate.Contains(target) && !target.Contains(candidate))) continue;
                    publisher = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                    version = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty;
                    uninstall = Convert.ToString(key.GetValue("QuietUninstallString")) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(uninstall)) uninstall = Convert.ToString(key.GetValue("UninstallString")) ?? string.Empty;
                    return;
                }
            }
        }

        private static void AddConfiguredStartupPaths(List<string> paths, string host, string valueName)
        {
            foreach (var view in Environment.Is64BitOperatingSystem ? new[] { RegistryView.Registry64, RegistryView.Registry32 } : new[] { RegistryView.Default })
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            using (var office = root.OpenSubKey("SOFTWARE\\Microsoft\\Office"))
            {
                if (office == null) continue;
                foreach (var version in office.GetSubKeyNames())
                using (var key = office.OpenSubKey(version + "\\" + host + "\\Options"))
                {
                    var configured = Convert.ToString(key == null ? null : key.GetValue(valueName));
                    if (!string.IsNullOrWhiteSpace(configured)) paths.Add(Environment.ExpandEnvironmentVariables(configured));
                }
            }
        }

        private static string ExtractMsi()
        {
            var resource = typeof(ProgramV110).Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("TarjimonOfficeUZSetup.msi", StringComparison.OrdinalIgnoreCase));
            if (resource == null) throw new FileNotFoundException("Embedded MSI topilmadi.");
            var path = Path.Combine(Path.GetTempPath(), "TarjimonOfficeUZ", Guid.NewGuid().ToString("N") + ".msi");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var input = typeof(ProgramV110).Assembly.GetManifestResourceStream(resource))
            using (var output = File.Create(path)) input.CopyTo(output);
            return path;
        }
    }

    internal sealed class ReviewForm : Form
    {
        private readonly ListView list = new ListView();
        public IEnumerable<AddinCandidate> SelectedItems { get { return list.CheckedItems.Cast<ListViewItem>().Select(x => (AddinCandidate)x.Tag); } }

        public ReviewForm(List<AddinCandidate> candidates)
        {
            Text = "Tarjimon Office UZ — Office tarjimonlari";
            Width = 980; Height = 560; MinimumSize = new Size(760, 440);
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false; MaximizeBox = true; FormBorderStyle = FormBorderStyle.Sizable;
            Font = new Font("Segoe UI", 9F); BackColor = Color.White;

            var header = new Panel { Dock = DockStyle.Top, Height = 112, BackColor = Color.White, Padding = new Padding(18, 14, 18, 8) };
            var brand = new Label { Dock = DockStyle.Top, Height = 34, Text = "Tarjimon Office UZ", Font = new Font("Segoe UI Semibold", 22F), ForeColor = Color.FromArgb(0, 82, 155) };
            var sub = new Label { Dock = DockStyle.Top, Height = 26, Text = "Office tarjimonlarini aniqlash", Font = new Font("Segoe UI", 11F), ForeColor = Color.FromArgb(0, 82, 155) };
            var info = new Label { Dock = DockStyle.Fill, Text = "Kompyuteringizda Office bilan bog'liq yoki funksional tarjimon/konvertorlar aniqlandi. Belgilanganlarini boshqarish mumkin.", ForeColor = Color.FromArgb(55, 55, 55), Padding = new Padding(0, 3, 0, 0) };
            header.Controls.Add(info); header.Controls.Add(sub); header.Controls.Add(brand);

            list.Dock = DockStyle.Fill; list.View = View.Details; list.CheckBoxes = true; list.FullRowSelect = true; list.GridLines = true;
            list.HideSelection = false; list.MultiSelect = true; list.BackColor = Color.White;
            list.Columns.Add("Mahsulot nomi", 220); list.Columns.Add("Ishlab chiqaruvchi", 165); list.Columns.Add("Versiya", 90); list.Columns.Add("Dastur", 110); list.Columns.Add("Ishonch", 85); list.Columns.Add("Muallif / ishlab chiqaruvchi", 210);
            foreach (var item in candidates)
            {
                var row = new ListViewItem(item.Product ?? "Noma'lum");
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Publisher) ? "Ishlab chiqaruvchi noma'lum" : item.Publisher);
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Version) ? "—" : item.Version);
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Host) ? "Office" : item.Host);
                row.SubItems.Add(Math.Min(100, item.Score) + "/100");
                row.SubItems.Add(ResolveDeveloper(item));
                row.Tag = item; row.Checked = item.IsOwnProduct;
                list.Items.Add(row);
            }

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 98, BackColor = Color.FromArgb(247, 249, 252), Padding = new Padding(18, 12, 18, 12) };
            var footerTitle = new Label { Dock = DockStyle.Top, Height = 26, Text = "Tarjimon Office UZ", Font = new Font("Segoe UI Semibold", 10F), ForeColor = Color.FromArgb(0, 82, 155) };
            var footerText = new Label { Dock = DockStyle.Fill, Text = "Mahsulotni o'rnatishdan oldin aniqlangan tarjimonlar ro'yxati.", ForeColor = Color.FromArgb(70, 70, 70) };
            footer.Controls.Add(footerText); footer.Controls.Add(footerTitle);

            var buttons = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.White, Padding = new Padding(18, 8, 18, 10) };
            var cancel = new Button { Text = "Bekor qilish", Width = 126, Height = 34, Anchor = AnchorStyles.Right | AnchorStyles.Top, DialogResult = DialogResult.Cancel };
            var ok = new Button { Text = "Tasdiqlash", Width = 134, Height = 34, Anchor = AnchorStyles.Right | AnchorStyles.Top, DialogResult = DialogResult.OK, BackColor = Color.FromArgb(30, 105, 190), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            cancel.Left = buttons.ClientSize.Width - cancel.Width; ok.Left = cancel.Left - 12 - ok.Width; cancel.Top = 8; ok.Top = 8;
            buttons.Resize += delegate { cancel.Left = buttons.ClientSize.Width - cancel.Width; ok.Left = cancel.Left - 12 - ok.Width; };
            buttons.Controls.Add(cancel); buttons.Controls.Add(ok);
            AcceptButton = ok; CancelButton = cancel;

            Controls.Add(list); Controls.Add(footer); Controls.Add(buttons); Controls.Add(header);
        }

        private static string ResolveDeveloper(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "Dostonjon Ashurov";
            return string.IsNullOrWhiteSpace(item.Publisher) ? "Aniqlanmagan" : item.Publisher;
        }
    }
}

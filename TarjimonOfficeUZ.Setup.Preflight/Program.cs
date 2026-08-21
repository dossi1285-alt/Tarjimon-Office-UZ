using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public bool IsOwnProduct { get; set; }
        public int Score { get; set; }
        public string Evidence { get; set; }
    }

    internal static class Program
    {
        private static readonly string[] FunctionWords =
        {
            "translit", "transliteration", "transliterator", "translator", "translation",
            "tarjimon", "переводчик", "перевод", "preslov", "preslovljav", "preslovljanje",
            "kirill", "kiril", "cyrillic", "кирилл", "lotin", "latin", "латин",
            "uzbek", "o'zbek", "узбек", "konvert", "converter", "conversion", "convert"
        };

        private static readonly string[] StrongFunctionPairs =
        {
            "kirill lotin", "lotin kirill", "cyrillic latin", "latin cyrillic",
            "cyrillic to latin", "latin to cyrillic", "kirill to latin", "latin to kirill",
            "kirill lotin converter", "lotin kirill converter", "translit word", "translit office",
            "word translit", "office translit", "preslovljanje", "preslovljavanje"
        };

        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var candidates = ScanCandidates();
                if (candidates.Count > 0)
                {
                    using (var form = new ReviewForm(candidates))
                    {
                        if (form.ShowDialog() != DialogResult.OK) return 1602;
                        foreach (var item in form.SelectedItems)
                        {
                            if (string.IsNullOrWhiteSpace(item.UninstallString))
                            {
                                MessageBox.Show("'" + item.Product + "' uchun qo'llab-quvvatlanadigan o'chirish buyrug'i topilmadi. U o'chirilmaydi.",
                                    "Tarjimon Office UZ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }
                            if (!RunUninstall(item.UninstallString)) return 1603;
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
            var discovered = new List<AddinCandidate>();
            var views = Environment.Is64BitOperatingSystem
                ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                : new[] { RegistryView.Default };

            ScanOfficeAddins(discovered, views);
            ScanWordStartup(discovered);
            ScanExcelStartup(discovered);
            ScanInstalledPrograms(discovered, views);

            return discovered
                .GroupBy(BuildProductIdentity, StringComparer.OrdinalIgnoreCase)
                .Select(MergeCandidate)
                .Where(x => x.IsOwnProduct || x.Score >= 30)
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
                    var loadBehavior = Convert.ToString(key.GetValue("LoadBehavior")) ?? string.Empty;
                    var text = NormalizeSearchText(keyName, friendly, description, manifest, progId, assembly);
                    var semantic = ScoreSemantic(text);
                    if (semantic <= 0) continue;

                    var score = semantic + 35;
                    if (!string.IsNullOrWhiteSpace(progId)) score += 5;
                    if (!string.IsNullOrWhiteSpace(manifest) || !string.IsNullOrWhiteSpace(assembly)) score += 5;
                    if (!string.IsNullOrWhiteSpace(loadBehavior)) score += 2;

                    AddCandidate(list, friendly, host,
                        hive + "\\" + view + "\\Office\\" + host + "\\Addins\\" + keyName,
                        text, keyName, score,
                        "Office " + host + " Addins registry; funksional metadata: " + BuildSemanticEvidence(text));
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
                    var semantic = ScoreSemantic(text);
                    var own = IsOwnProduct(text, publisher);
                    var officeAssociation = ContainsOfficeAssociation(installLocation, displayIcon, url);
                    var functionalFileEvidence = FindFunctionalFileEvidence(installLocation);

                    if (!own && semantic <= 0 && string.IsNullOrWhiteSpace(functionalFileEvidence)) continue;
                    if (!own && !officeAssociation && string.IsNullOrWhiteSpace(functionalFileEvidence)) continue;

                    var score = own ? Math.Max(semantic + 15, 90) : semantic + 15;
                    if (officeAssociation) score += 20;
                    if (!string.IsNullOrWhiteSpace(functionalFileEvidence)) score += 25;
                    if (score < 30 && !own) continue;

                    var evidence = "Windows Uninstall registry";
                    if (officeAssociation) evidence += "; Office bilan bog'liq yo'l/metadata";
                    if (!string.IsNullOrWhiteSpace(functionalFileEvidence)) evidence += "; funksional fayl dalili: " + functionalFileEvidence;
                    if (own) evidence += "; Tarjimon Office UZ own-product identity";
                    else evidence += "; funksional metadata: " + BuildSemanticEvidence(text);

                    list.Add(new AddinCandidate
                    {
                        Product = display,
                        Publisher = publisher,
                        Version = version,
                        Host = officeAssociation ? "Office/Windows" : "Windows",
                        Registration = hive + "\\" + view + "\\Uninstall\\" + name,
                        UninstallString = uninstall,
                        IsOwnProduct = own,
                        Score = Math.Min(score, 100),
                        Evidence = evidence
                    });
                }
            }
        }

        private static bool ContainsOfficeAssociation(params string[] values)
        {
            var text = NormalizeSearchText(values);
            return text.Contains("microsoft office") || text.Contains("office\\") || text.Contains("\\office") ||
                   text.Contains("microsoft\\word") || text.Contains("microsoft\\excel") || text.Contains("\\word\\") ||
                   text.Contains("\\excel\\") || text.Contains("startup") || text.Contains("xlstart") ||
                   text.Contains("\\addins\\") || text.Contains("\\addin\\");
        }

        private static string FindFunctionalFileEvidence(string installLocation)
        {
            if (string.IsNullOrWhiteSpace(installLocation) || !Directory.Exists(installLocation)) return string.Empty;
            try
            {
                var files = new List<string>(Directory.GetFiles(installLocation));
                foreach (var subdir in Directory.GetDirectories(installLocation).Take(12))
                {
                    try { files.AddRange(Directory.GetFiles(subdir)); } catch { }
                    if (files.Count >= 240) break;
                }

                var hits = new List<string>();
                foreach (var file in files.Take(240))
                {
                    var extension = Path.GetExtension(file) ?? string.Empty;
                    if (!new[] { ".dot", ".dotm", ".dotx", ".wll", ".xla", ".xlam", ".xll", ".dll", ".exe", ".vsto", ".xml" }.Contains(extension, StringComparer.OrdinalIgnoreCase)) continue;
                    var text = NormalizeSearchText(Path.GetFileName(file), ReadFileMetadata(file));
                    var semantic = ScoreSemantic(text);
                    if (semantic <= 0) continue;
                    hits.Add(Path.GetFileName(file) + " [" + BuildSemanticEvidence(text) + "]");
                    if (hits.Count >= 3) break;
                }
                return string.Join(", ", hits);
            }
            catch { return string.Empty; }
        }

        private static int ScoreSemantic(string text)
        {
            var normalized = NormalizeSearchText(text);
            if (string.IsNullOrWhiteSpace(normalized)) return 0;

            var strong = StrongFunctionPairs.Count(normalized.Contains);
            var matches = FunctionWords.Where(normalized.Contains).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (strong > 0) return 35 + Math.Min(strong, 2) * 5;
            if (matches.Contains("translit") || matches.Contains("transliteration") || matches.Contains("transliterator")) return 25 + Math.Min(matches.Count - 1, 2) * 5;
            if (matches.Contains("translator") || matches.Contains("translation") || matches.Contains("tarjimon") || matches.Contains("переводчик") || matches.Contains("перевод")) return 25 + Math.Min(matches.Count - 1, 2) * 5;
            if (matches.Count >= 2 && (matches.Contains("kirill") || matches.Contains("cyrillic") || matches.Contains("lotin") || matches.Contains("latin"))) return 25;
            if (matches.Count >= 3) return 20;
            return 0;
        }

        private static string BuildSemanticEvidence(string text)
        {
            var normalized = NormalizeSearchText(text);
            var matches = FunctionWords.Where(normalized.Contains).Distinct(StringComparer.OrdinalIgnoreCase).Take(5).ToArray();
            return matches.Length == 0 ? "Office funksional signali topilmadi" : string.Join(", ", matches);
        }

        private static void AddCandidate(List<AddinCandidate> list, string product, string host, string registration, string searchText, string keyName, int score, string evidence)
        {
            string publisher, version, uninstall;
            FindUninstall(product, keyName, out publisher, out version, out uninstall);
            list.Add(new AddinCandidate
            {
                Product = product,
                Publisher = publisher,
                Version = version,
                Host = host,
                Registration = registration,
                UninstallString = uninstall,
                IsOwnProduct = IsOwnProduct(searchText, publisher),
                Score = Math.Min(score, 100),
                Evidence = evidence
            });
        }

        private static void ScanWordStartup(List<AddinCandidate> list)
        {
            var paths = new List<string>();
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrWhiteSpace(appData))
            {
                paths.Add(Path.Combine(appData, "Microsoft", "Word", "STARTUP"));
                paths.Add(Path.Combine(appData, "Microsoft", "Templates"));
            }
            AddOfficeVersionStartupPaths(paths, "Word", "STARTUP");
            AddConfiguredStartupPaths(paths, "Word", "STARTUP-PATH");
            foreach (var path in paths.Distinct(StringComparer.OrdinalIgnoreCase)) ScanFileDirectory(list, path, "Word", "Word Startup");
        }

        private static void ScanExcelStartup(List<AddinCandidate> list)
        {
            var paths = new List<string>();
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrWhiteSpace(appData)) paths.Add(Path.Combine(appData, "Microsoft", "Excel", "XLSTART"));
            AddOfficeVersionStartupPaths(paths, "Excel", "XLSTART");
            AddConfiguredStartupPaths(paths, "Excel", "OPEN");
            foreach (var path in paths.Distinct(StringComparer.OrdinalIgnoreCase)) ScanFileDirectory(list, path, "Excel", "Excel Startup");
        }

        private static void AddOfficeVersionStartupPaths(List<string> paths, string host, string folder)
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
                    var configured = Convert.ToString(key == null ? null : key.GetValue(folder == "XLSTART" ? "OPEN" : "STARTUP-PATH"));
                    if (!string.IsNullOrWhiteSpace(configured)) paths.Add(Environment.ExpandEnvironmentVariables(configured));
                }
            }

            var programFiles = new[] { Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) };
            foreach (var pf in programFiles.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                paths.Add(Path.Combine(pf, "Microsoft Office", "root", "Office16", folder));
                paths.Add(Path.Combine(pf, "Microsoft Office", "Office16", folder));
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

        private static void ScanFileDirectory(List<AddinCandidate> list, string directory, string host, string locationName)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return;
            string[] files;
            try { files = Directory.GetFiles(directory); } catch { return; }
            foreach (var file in files)
            {
                var name = Path.GetFileName(file) ?? string.Empty;
                var extension = Path.GetExtension(file) ?? string.Empty;
                if (!new[] { ".dotm", ".dot", ".dotx", ".wll", ".xlam", ".xla", ".xll" }.Contains(extension, StringComparer.OrdinalIgnoreCase)) continue;
                var text = NormalizeSearchText(name, file, ReadFileMetadata(file));
                var semantic = ScoreSemantic(text);
                if (semantic <= 0) continue;
                string publisher, version, uninstall;
                FindUninstall(name, name, out publisher, out version, out uninstall);
                list.Add(new AddinCandidate
                {
                    Product = Path.GetFileNameWithoutExtension(name), Publisher = publisher, Version = version,
                    Host = host, Registration = locationName + ": " + file, UninstallString = uninstall,
                    IsOwnProduct = IsOwnProduct(text, publisher), Score = Math.Min(semantic + 35, 100),
                    Evidence = locationName + " + Office startup fayli; funksional metadata: " + BuildSemanticEvidence(text)
                });
            }
        }

        private static string ReadFileMetadata(string file)
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(file);
                return string.Join(" ", info.ProductName, info.CompanyName, info.FileDescription, info.InternalName, info.OriginalFilename);
            }
            catch { return string.Empty; }
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

        private static string BuildProductIdentity(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";
            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;
            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);
            if (!string.IsNullOrWhiteSpace(publisher)) return "APP:" + product + "|" + publisher;
            return "APP:" + product;
        }

        private static AddinCandidate MergeCandidate(IGrouping<string, AddinCandidate> group)
        {
            var first = group.OrderByDescending(x => x.Score).First();
            first.Score = group.Max(x => x.Score);
            first.IsOwnProduct = group.Any(x => x.IsOwnProduct);
            first.Host = string.Join(", ", group.Select(x => x.Host).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x));
            first.UninstallString = group.Select(x => x.UninstallString).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.UninstallString;
            first.Publisher = group.Select(x => x.Publisher).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.Publisher;
            first.Version = group.Select(x => x.Version).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.Version;
            first.Evidence = string.Join("; ", group.Select(x => x.Evidence).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Take(3));
            return first;
        }

        internal static string ResolveDeveloper(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "Dostonjon Ashurov";
            return string.IsNullOrWhiteSpace(item.Publisher) ? "Aniqlanmagan" : item.Publisher;
        }

        internal static string DisplayHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host)) return string.Empty;
            var parts = host.Split(new[] { ',', '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !x.Equals("Windows", StringComparison.OrdinalIgnoreCase) &&
                            !x.Equals("Office", StringComparison.OrdinalIgnoreCase) &&
                            !x.Equals("Office/Windows", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            return string.Join(", ", parts);
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
            normalized = normalized.Replace("_", " ").Replace("-", " ");
            normalized = Regex.Replace(normalized, @"[\\./]+", " ");
            normalized = Regex.Replace(normalized, @"\s+(word|excel)$", "");
            return normalized.Trim();
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

        private static bool RunUninstall(string commandLine)
        {
            string fileName, arguments;
            var trimmed = commandLine.TrimStart();
            if (trimmed.StartsWith("msiexec", StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith("msiexec.exe", StringComparison.OrdinalIgnoreCase))
            {
                fileName = "msiexec.exe";
                var space = trimmed.IndexOf(' ');
                arguments = space >= 0 ? trimmed.Substring(space + 1).Trim() : string.Empty;
                arguments = Regex.Replace(arguments, @"(?i)(^|\s)/(i|x)(?=\s*\{)", "$1/X");
                if (!Regex.IsMatch(arguments, @"(?i)(^|\s)/x(?=\s*\{)")) return false;
            }
            else if (trimmed.StartsWith("\""))
            {
                var end = trimmed.IndexOf('"', 1); fileName = end > 0 ? trimmed.Substring(1, end - 1) : trimmed.Trim('"'); arguments = end > 0 ? trimmed.Substring(end + 1).Trim() : string.Empty;
            }
            else
            {
                var split = trimmed.IndexOf(' '); fileName = split > 0 ? trimmed.Substring(0, split) : trimmed; arguments = split > 0 ? trimmed.Substring(split + 1) : string.Empty;
            }
            using (var p = Process.Start(new ProcessStartInfo { FileName = fileName, Arguments = arguments, UseShellExecute = true, Verb = "runas" }))
            {
                if (p == null) return false; p.WaitForExit(); return p.ExitCode == 0 || p.ExitCode == 3010 || p.ExitCode == 1641;
            }
        }

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
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;

            var header = new Panel { Dock = DockStyle.Top, Height = 112, BackColor = Color.White, Padding = new Padding(18, 14, 18, 8) };
            var brand = new Label
            {
                Dock = DockStyle.Top, Height = 42,
                Text = "Tarjimon Office UZ",
                Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 75, 135),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var subtitle = new Label
            {
                Dock = DockStyle.Top, Height = 27,
                Text = "Office tarjimonlarini aniqlash",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(55, 105, 170),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var description = new Label
            {
                Dock = DockStyle.Fill,
                Text = "",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(75, 75, 75),
                TextAlign = ContentAlignment.MiddleLeft
            };
            header.Controls.Add(description); header.Controls.Add(subtitle); header.Controls.Add(brand);

            list.View = View.Details; list.CheckBoxes = true; list.FullRowSelect = true; list.GridLines = false;
            list.MultiSelect = true; list.HideSelection = false; list.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            list.BorderStyle = BorderStyle.FixedSingle;
            list.BackColor = Color.White;
            list.Columns.Add("Mahsulot nomi", 225); list.Columns.Add("Ishlab chiqaruvchi", 175);
            list.Columns.Add("Versiya", 75); list.Columns.Add("Dastur", 115); list.Columns.Add("Ishonch", 70); list.Columns.Add("Muallif / ishlab chiquvchi", 350);
            foreach (var item in candidates)
            {
                var row = new ListViewItem(string.IsNullOrWhiteSpace(item.Product) ? "Noma'lum" : item.Product);
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Publisher) ? "Ishlab chiqaruvchi noma'lum" : item.Publisher);
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Version) ? "—" : item.Version);
                row.SubItems.Add(Program.DisplayHost(item.Host));
                row.SubItems.Add(item.Score + "/100");
                row.SubItems.Add(Program.ResolveDeveloper(item));
                row.Tag = item; row.Checked = item.IsOwnProduct; list.Items.Add(row);
            }

            var productCard = new Panel
            {
                Dock = DockStyle.Bottom, Height = 66,
                BackColor = Color.FromArgb(244, 248, 253),
                Padding = new Padding(14, 7, 14, 7)
            };
            var productName = new Label
            {
                Dock = DockStyle.Top, Height = 25,
                Text = "Tarjimon Office UZ",
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 75, 135),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var productHint = new Label
            {
                Dock = DockStyle.Fill,
                Text = "",
                Font = new Font("Segoe UI", 8.8F),
                ForeColor = Color.FromArgb(80, 90, 105),
                TextAlign = ContentAlignment.MiddleLeft
            };
            productCard.Controls.Add(productHint); productCard.Controls.Add(productName);

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 58, Padding = new Padding(8, 7, 8, 8), BackColor = Color.White };
            var cancel = new Button { Text = "Bekor qilish", Width = 125, Height = 34, DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            var confirm = new Button { Text = "Tasdiqlash", Width = 135, Height = 34, DialogResult = DialogResult.OK, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            confirm.BackColor = Color.FromArgb(25, 105, 190); confirm.ForeColor = Color.White; confirm.FlatStyle = FlatStyle.Flat; confirm.FlatAppearance.BorderSize = 0;
            cancel.FlatStyle = FlatStyle.Flat; cancel.FlatAppearance.BorderColor = Color.FromArgb(205, 210, 218);
            Action layoutButtons = () => { cancel.Left = panel.ClientSize.Width - cancel.Width - 10; confirm.Left = cancel.Left - confirm.Width - 10; cancel.Top = 7; confirm.Top = 7; };
            panel.Resize += (s, e) => layoutButtons(); panel.Controls.Add(cancel); panel.Controls.Add(confirm); layoutButtons();

            Controls.Add(list); Controls.Add(header); Controls.Add(productCard); Controls.Add(panel);
            Resize += (s, e) => LayoutList();
            LayoutList();
            AcceptButton = confirm; CancelButton = cancel;

            void LayoutList()
            {
                var bottomReserved = productCard.Height + panel.Height;
                list.Left = 18;
                list.Top = header.Height + 6;
                list.Width = Math.Max(300, ClientSize.Width - 36);
                list.Height = Math.Max(120, ClientSize.Height - list.Top - bottomReserved - 8);
            }
        }
    }
}

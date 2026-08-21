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
    }

    internal static class Program
    {
        private static readonly string[] TranslatorWords =
        {
            "tarjimon", "translator", "translation", "translate", "language", "lingua",
            "перевод", "переводчик", "kl office", "kloffice", "kl_office", "office uz",
            "print kito", "print_kito", "kirill", "kiril", "kyril", "lotin", "latin",
            "o'zbek", "uzbek", "узбек"
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

            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            foreach (var host in new[] { "Word", "Excel" })
            {
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
                        if (IsTranslatorCandidate(text))
                            AddCandidate(discovered, friendly, host, hive + "\\" + view + "\\Office\\Addins\\" + keyName, text, keyName);
                    }
                }
            }

            ScanWordStartup(discovered);
            ScanExcelStartup(discovered);
            ScanOfficeComRegistrations(discovered, views);

            return discovered
                .GroupBy(BuildProductIdentity, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    var first = g.First();
                    first.Host = string.Join(", ", g.Select(x => x.Host).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x));
                    first.IsOwnProduct = g.Any(x => x.IsOwnProduct);
                    first.UninstallString = g.Select(x => x.UninstallString).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.UninstallString;
                    first.Publisher = g.Select(x => x.Publisher).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.Publisher;
                    first.Version = g.Select(x => x.Version).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? first.Version;
                    return first;
                })
                .OrderByDescending(x => x.IsOwnProduct)
                .ThenBy(x => x.Product)
                .ToList();
        }

        private static bool IsTranslatorCandidate(string text)
        {
            return TranslatorWords.Any(text.Contains);
        }

        private static void AddCandidate(List<AddinCandidate> list, string product, string host, string registration, string searchText, string keyName)
        {
            string publisher, version;
            var uninstall = FindUninstall(product, keyName, out publisher, out version);
            list.Add(new AddinCandidate
            {
                Product = product,
                Publisher = publisher,
                Version = version,
                Host = host,
                Registration = registration,
                UninstallString = uninstall,
                IsOwnProduct = IsOwnProduct(searchText, publisher)
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
            foreach (var path in paths.Distinct(StringComparer.OrdinalIgnoreCase))
                ScanFileDirectory(list, path, "Word", "Word Startup");
        }

        private static void ScanExcelStartup(List<AddinCandidate> list)
        {
            var paths = new List<string>();
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrWhiteSpace(appData)) paths.Add(Path.Combine(appData, "Microsoft", "Excel", "XLSTART"));
            AddOfficeVersionStartupPaths(paths, "Excel", "XLSTART");
            AddConfiguredStartupPaths(paths, "Excel", "OPEN");
            foreach (var path in paths.Distinct(StringComparer.OrdinalIgnoreCase))
                ScanFileDirectory(list, path, "Excel", "Excel Startup");
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
                {
                    using (var key = office.OpenSubKey(version + "\\" + host + "\\Options"))
                    {
                        var configured = Convert.ToString(key == null ? null : key.GetValue(folder == "XLSTART" ? "OPEN" : "STARTUP-PATH"));
                        if (!string.IsNullOrWhiteSpace(configured)) paths.Add(Environment.ExpandEnvironmentVariables(configured));
                    }
                }
            }

            var programFiles = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };
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
                if (!IsTranslatorCandidate(text)) continue;
                string publisher, version;
                var uninstall = FindUninstall(name, name, out publisher, out version);
                list.Add(new AddinCandidate
                {
                    Product = Path.GetFileNameWithoutExtension(name), Publisher = publisher, Version = version,
                    Host = host, Registration = locationName + ": " + file, UninstallString = uninstall,
                    IsOwnProduct = IsOwnProduct(text, publisher)
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

        private static void ScanOfficeComRegistrations(List<AddinCandidate> list, RegistryView[] views)
        {
            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            using (var classes = root.OpenSubKey("Software\\Classes\\CLSID"))
            {
                if (classes == null) continue;
                foreach (var clsid in classes.GetSubKeyNames())
                {
                    try
                    {
                        using (var key = classes.OpenSubKey(clsid))
                        {
                            if (key == null) continue;
                            var display = Convert.ToString(key.GetValue(null)) ?? string.Empty;
                            var progKey = key.OpenSubKey("ProgID");
                            var progId = Convert.ToString(progKey == null ? null : progKey.GetValue(null)) ?? string.Empty;
                            if (progKey != null) progKey.Dispose();
                            var text = NormalizeSearchText(display, progId, clsid);
                            using (var inproc = key.OpenSubKey("InprocServer32")) text += " " + Convert.ToString(inproc == null ? null : inproc.GetValue(null));
                            using (var local = key.OpenSubKey("LocalServer32")) text += " " + Convert.ToString(local == null ? null : local.GetValue(null));
                            if (!IsTranslatorCandidate(NormalizeSearchText(text))) continue;
                            AddCandidate(list, string.IsNullOrWhiteSpace(display) ? progId : display, "Word/Excel", "CLSID:" + clsid, text, clsid);
                        }
                    }
                    catch { }
                }
            }
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
            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;

            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";

            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);
            if (!string.IsNullOrWhiteSpace(publisher)) return "APP:" + product + "|" + publisher;
            return "APP:" + product;
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
            normalized = Regex.Replace(normalized, @"\s+uz$", " uz");
            return normalized.Trim();
        }

        private static string FindUninstall(string friendly, string keyName, out string publisher, out string version)
        {
            publisher = string.Empty; version = string.Empty;
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
                    var uninstall = Convert.ToString(key.GetValue("UninstallString")) ?? string.Empty;
                    var pub = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                    var ver = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty;
                    var candidate = NormalizeSearchText(display, pub, name);
                    if (!string.IsNullOrWhiteSpace(uninstall) && (candidate.Contains((friendly ?? string.Empty).ToLowerInvariant()) || candidate.Contains((keyName ?? string.Empty).ToLowerInvariant())))
                    { publisher = pub; version = ver; return uninstall; }
                }
            }
            return string.Empty;
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
            Text = "Tarjimon Office UZ — mavjud Office tarjimonlari";
            Width = 760; Height = 420; MinimumSize = new Size(620, 360);
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false; MaximizeBox = true; FormBorderStyle = FormBorderStyle.Sizable;
            Font = new Font("Segoe UI", 9F);

            var title = new Label
            {
                Dock = DockStyle.Top, Height = 76,
                Text = "Kompyuterda mavjud Office tarjimon/add-inlar aniqlandi.\r\nO'chiriladiganlarini belgilang. Belgilanmaganlari saqlanadi.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), Padding = new Padding(16, 12, 16, 6), TextAlign = ContentAlignment.MiddleLeft
            };

            list.View = View.Details; list.CheckBoxes = true; list.FullRowSelect = true; list.GridLines = true;
            list.MultiSelect = true; list.HideSelection = false; list.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            list.Columns.Add("Qo'shimcha nomi", 235); list.Columns.Add("Mahsulot / ishlab chiqaruvchi", 245);
            list.Columns.Add("Versiya", 75); list.Columns.Add("Dastur", 100);
            foreach (var item in candidates)
            {
                var row = new ListViewItem(item.Product);
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Publisher) ? "Ishlab chiqaruvchi noma'lum" : item.Publisher);
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Version) ? "—" : item.Version);
                row.SubItems.Add(item.Host); row.Tag = item; row.Checked = item.IsOwnProduct; list.Items.Add(row);
            }

            var info = new Label
            {
                Dock = DockStyle.Bottom, Height = 58,
                Text = "Faqat ro'yxatda ko'rsatilgan va qo'llab-quvvatlanadigan o'chirish mexanizmi mavjud mahsulotlar olib tashlanadi.\r\nUchinchi tomon add-inlari aniqlansa, ular belgilanmaydi — foydalanuvchi o'zi tanlaydi.",
                Padding = new Padding(16, 7, 16, 4), ForeColor = Color.DarkSlateGray, TextAlign = ContentAlignment.MiddleLeft
            };

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(8, 5, 8, 7) };
            var cancel = new Button { Text = "Bekor qilish", Width = 120, Height = 34, DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            var confirm = new Button { Text = "Tasdiqlash", Width = 135, Height = 34, DialogResult = DialogResult.OK, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            Action layoutButtons = () => { cancel.Left = panel.ClientSize.Width - cancel.Width; confirm.Left = cancel.Left - confirm.Width - 8; cancel.Top = 5; confirm.Top = 5; };
            panel.Resize += (s, e) => layoutButtons(); panel.Controls.Add(cancel); panel.Controls.Add(confirm); layoutButtons();

            Controls.Add(list); Controls.Add(title); Controls.Add(info); Controls.Add(panel);
            Resize += (s, e) => LayoutList();
            LayoutList();
            AcceptButton = confirm; CancelButton = cancel;

            void LayoutList()
            {
                var bottomReserved = info.Height + panel.Height;
                var top = title.Height;
                list.Left = 16;
                list.Top = top + 24;
                list.Width = Math.Max(200, ClientSize.Width - 32);
                list.Height = Math.Max(80, ClientSize.Height - list.Top - bottomReserved - 8);
            }
        }
    }
}
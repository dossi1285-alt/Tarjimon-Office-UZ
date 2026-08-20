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

        public override string ToString()
        {
            var owner = string.IsNullOrWhiteSpace(Publisher) ? "Publisher unknown" : Publisher;
            var version = string.IsNullOrWhiteSpace(Version) ? "Version unknown" : Version;
            return $"{Product}  |  {owner}  |  {version}  |  {Host}";
        }
    }

    internal static class Program
    {
        private static readonly string[] TranslatorWords =
        {
            "tarjimon", "translator", "translation", "translate", "language",
            "lingua", "перевод", "переводчик", "kl office", "office uz"
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
                        if (form.ShowDialog() != DialogResult.OK)
                            return 1602;

                        foreach (var item in form.SelectedItems)
                        {
                            if (string.IsNullOrWhiteSpace(item.UninstallString))
                            {
                                MessageBox.Show(
                                    $"'{item.Product}' uchun qo'llab-quvvatlanadigan uninstall buyrug'i topilmadi. U o'chirilmaydi.",
                                    "Tarjimon Office UZ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }

                            if (!RunUninstall(item.UninstallString))
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
            var result = new List<AddinCandidate>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var views = Environment.Is64BitOperatingSystem
                ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                : new[] { RegistryView.Default };

            foreach (var view in views)
            {
                foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
                {
                    foreach (var host in new[] { "Word", "Excel" })
                    {
                        using (var root = RegistryKey.OpenBaseKey(hive, view))
                        using (var addins = root.OpenSubKey($"SOFTWARE\\Microsoft\\Office\\{host}\\Addins"))
                        {
                            if (addins == null) continue;
                            foreach (var keyName in addins.GetSubKeyNames())
                            {
                                using (var key = addins.OpenSubKey(keyName))
                                {
                                    if (key == null) continue;
                                    var friendly = Convert.ToString(key.GetValue("FriendlyName")) ?? keyName;
                                    var description = Convert.ToString(key.GetValue("Description")) ?? string.Empty;
                                    var manifest = Convert.ToString(key.GetValue("Manifest")) ?? string.Empty;
                                    var text = (keyName + " " + friendly + " " + description + " " + manifest).ToLowerInvariant();
                                    if (!TranslatorWords.Any(text.Contains)) continue;

                                    var uninstall = FindUninstall(friendly, keyName, out var publisher, out var version);
                                    var own = text.Contains("tarjimon office uz") || text.Contains("tarjimonofficeuz");
                                    var registration = $"{hive}\\{view}\\SOFTWARE\\Microsoft\\Office\\{host}\\Addins\\{keyName}";
                                    var signature = registration + "|" + friendly;
                                    if (!seen.Add(signature)) continue;

                                    result.Add(new AddinCandidate
                                    {
                                        Product = friendly,
                                        Publisher = publisher,
                                        Version = version,
                                        Host = host,
                                        Registration = registration,
                                        UninstallString = uninstall,
                                        IsOwnProduct = own
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return result.OrderByDescending(x => x.IsOwnProduct).ThenBy(x => x.Product).ToList();
        }

        private static string FindUninstall(string friendly, string keyName, out string publisher, out string version)
        {
            publisher = string.Empty;
            version = string.Empty;
            var views = Environment.Is64BitOperatingSystem
                ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                : new[] { RegistryView.Default };

            foreach (var view in views)
            {
                foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
                {
                    using (var root = RegistryKey.OpenBaseKey(hive, view))
                    using (var parent = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                    {
                        if (parent == null) continue;
                        foreach (var name in parent.GetSubKeyNames())
                        {
                            using (var key = parent.OpenSubKey(name))
                            {
                                if (key == null) continue;
                                var display = Convert.ToString(key.GetValue("DisplayName")) ?? string.Empty;
                                var uninstall = Convert.ToString(key.GetValue("UninstallString")) ?? string.Empty;
                                var pub = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                                var ver = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty;
                                var candidateText = (display + " " + pub + " " + name).ToLowerInvariant();
                                if (!string.IsNullOrWhiteSpace(uninstall) &&
                                    (candidateText.Contains(friendly.ToLowerInvariant()) ||
                                     candidateText.Contains(keyName.ToLowerInvariant()) ||
                                     TranslatorWords.Any(candidateText.Contains)))
                                {
                                    publisher = pub;
                                    version = ver;
                                    return uninstall;
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        private static bool RunUninstall(string commandLine)
        {
            string fileName;
            string arguments;
            var trimmed = commandLine.TrimStart();

            if (trimmed.StartsWith("msiexec", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("msiexec.exe", StringComparison.OrdinalIgnoreCase))
            {
                fileName = "msiexec.exe";
                arguments = trimmed.Substring(trimmed.IndexOf(' ') >= 0 ? trimmed.IndexOf(' ') + 1 : trimmed.Length).Trim();

                // Windows uninstall registrations commonly use either /I{PRODUCT-CODE}
                // or /I {PRODUCT-CODE}. Both mean maintenance/install mode. For an
                // uninstall request we must reliably convert the operation to /X.
                arguments = Regex.Replace(
                    arguments,
                    @"(?i)(^|\s)/(i|x)(?=\s*\{)",
                    "$1/X");

                if (!Regex.IsMatch(arguments, @"(?i)(^|\s)/x(?=\s*\{)"))
                {
                    // Do not launch msiexec with an ambiguous command line.
                    // If the registered string has no product-code uninstall target,
                    // report it as unsupported instead of showing the MSI help dialog.
                    return false;
                }
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

            using (var p = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = true,
                Verb = "runas"
            }))
            {
                if (p == null) return false;
                p.WaitForExit();
                return p.ExitCode == 0 || p.ExitCode == 3010 || p.ExitCode == 1641;
            }
        }

        private static string ExtractMsi()
        {
            var resource = typeof(Program).Assembly.GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith("TarjimonOfficeUZSetup.msi", StringComparison.OrdinalIgnoreCase));
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
        public IEnumerable<AddinCandidate> SelectedItems => list.CheckedItems.Cast<ListViewItem>().Select(x => (AddinCandidate)x.Tag);

        public ReviewForm(List<AddinCandidate> candidates)
        {
            Text = "Tarjimon Office UZ — mavjud Office tarjimonlari";
            Width = 760;
            Height = 420;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Font = new Font("Segoe UI", 9F);

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 76,
                Text = "Kompyuterda mavjud Office tarjimon/add-inlar aniqlandi.\r\nO'chiriladiganlarini belgilang. Belgilanmaganlari saqlanadi.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Padding = new Padding(16, 12, 16, 6),
                TextAlign = ContentAlignment.MiddleLeft
            };

            list.View = View.Details;
            list.CheckBoxes = true;
            list.FullRowSelect = true;
            list.GridLines = true;
            list.MultiSelect = true;
            list.HideSelection = false;
            list.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            list.Columns.Add("Qo'shimcha nomi", 245);
            list.Columns.Add("Mahsulot nomi", 170);
            list.Columns.Add("Versiya", 75);
            list.Columns.Add("Dastur", 100);

            foreach (var item in candidates)
            {
                var row = new ListViewItem(item.Product);
                row.SubItems.Add("Tarjimon Office UZ");
                row.SubItems.Add(string.IsNullOrWhiteSpace(item.Version) ? "—" : item.Version);
                row.SubItems.Add(item.Host);
                row.Tag = item;
                row.Checked = item.IsOwnProduct;
                list.Items.Add(row);
            }

            var info = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                Text = "Faqat ro'yxatda ko'rsatilgan va qo'llab-quvvatlanadigan uninstall mexanizmi mavjud mahsulotlar olib tashlanadi.\r\nUchinchi tomon add-inlari roziliksiz o'chirilmaydi.",
                Padding = new Padding(16, 7, 16, 4),
                ForeColor = Color.DarkSlateGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(8, 5, 8, 7) };
            var cancel = new Button
            {
                Text = "Bekor qilish",
                Width = 120,
                Height = 34,
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            var confirm = new Button
            {
                Text = "Tasdiqlash",
                Width = 135,
                Height = 34,
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            cancel.Left = panel.ClientSize.Width - cancel.Width;
            confirm.Left = cancel.Left - confirm.Width - 8;
            cancel.Top = 5;
            confirm.Top = 5;
            cancel.BringToFront();
            confirm.BringToFront();
            panel.Resize += (s, e) =>
            {
                cancel.Left = panel.ClientSize.Width - cancel.Width;
                confirm.Left = cancel.Left - confirm.Width - 8;
            };
            panel.Controls.Add(cancel);
            panel.Controls.Add(confirm);

            list.Location = new Point(16, 100);
            list.Size = new Size(712, 200);
            list.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Controls.Add(list);
            Controls.Add(title);
            Controls.Add(info);
            Controls.Add(panel);

            AcceptButton = confirm;
            CancelButton = cancel;
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TarjimonOfficeUZ.Setup.Preflight
{
    internal static class InstallerFlow
    {
        private const string ProductName = "Tarjimon Office UZ";
        private const string ProductPublisher = "Dostonjon Ashurov";
        private const string DefaultFolderName = "Tarjimon Office UZ";
        private const string UninstallerResourceSuffix = "TarjimonOfficeUZUninstaller.exe";
        private const string LicenseResourceSuffix = "TarjimonOfficeUZLicense.rtf";

        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var own = FindOwnProduct();
                var requirements = CheckRequirements();
                using (var wizard = new InstallerWizard(own, requirements))
                    return wizard.ShowDialog() == DialogResult.OK ? 0 : 1602;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ProductName + " — Installer xatosi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1603;
            }
        }

        private static InstalledOwnProduct FindOwnProduct()
        {
            var views = Environment.Is64BitOperatingSystem
                ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                : new[] { RegistryView.Default };

            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            using (var uninstall = root.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
            {
                if (uninstall == null) continue;
                foreach (var keyName in uninstall.GetSubKeyNames())
                using (var key = uninstall.OpenSubKey(keyName))
                {
                    if (key == null) continue;
                    var displayName = Convert.ToString(key.GetValue("DisplayName")) ?? string.Empty;
                    if (!displayName.Equals(ProductName, StringComparison.OrdinalIgnoreCase)) continue;
                    var publisher = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(publisher) && !publisher.Equals(ProductPublisher, StringComparison.OrdinalIgnoreCase)) continue;
                    return new InstalledOwnProduct
                    {
                        DisplayVersion = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty,
                        InstallLocation = Convert.ToString(key.GetValue("InstallLocation")) ?? string.Empty
                    };
                }
            }
            return null;
        }

        private static RequirementStatus CheckRequirements()
        {
            return new RequirementStatus
            {
                Windows64 = Environment.Is64BitOperatingSystem,
                Word = IsOfficeHostInstalled("Word"),
                Excel = IsOfficeHostInstalled("Excel")
            };
        }

        private static bool IsOfficeHostInstalled(string host)
        {
            var views = Environment.Is64BitOperatingSystem
                ? new[] { RegistryView.Registry64, RegistryView.Registry32 }
                : new[] { RegistryView.Default };
            foreach (var view in views)
            foreach (var hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
            using (var root = RegistryKey.OpenBaseKey(hive, view))
            {
                if (root.OpenSubKey("SOFTWARE\\Microsoft\\Office\\16.0\\" + host) != null) return true;
                if (root.OpenSubKey("SOFTWARE\\Microsoft\\Office\\15.0\\" + host) != null) return true;
                if (root.OpenSubKey("SOFTWARE\\Microsoft\\Office\\14.0\\" + host) != null) return true;
                using (var clickToRun = root.OpenSubKey("SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration"))
                {
                    var ids = clickToRun == null ? string.Empty : Convert.ToString(clickToRun.GetValue("ProductReleaseIds")) ?? string.Empty;
                    if (ids.IndexOf("Office", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ids.IndexOf("Word", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ids.IndexOf("Excel", StringComparison.OrdinalIgnoreCase) >= 0) return true;
                }
            }
            return false;
        }

        private static string ExtractEmbedded(string suffix, string extension)
        {
            var resource = typeof(InstallerFlow).Assembly.GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
            if (resource == null) throw new FileNotFoundException("Embedded resource topilmadi: " + suffix);
            var directory = Path.Combine(Path.GetTempPath(), "TarjimonOfficeUZ", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, Guid.NewGuid().ToString("N") + extension);
            using (var input = typeof(InstallerFlow).Assembly.GetManifestResourceStream(resource))
            {
                if (input == null) throw new FileNotFoundException("Embedded resource topilmadi: " + suffix);
                using (var output = File.Create(path)) input.CopyTo(output);
            }
            return path;
        }

        private static string ReadEmbeddedText(string suffix)
        {
            var resource = typeof(InstallerFlow).Assembly.GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
            if (resource == null) return string.Empty;
            using (var input = typeof(InstallerFlow).Assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(input))
                return reader.ReadToEnd();
        }

        private static int RunMsiInstall(string msiPath, string folder)
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = "/i \"" + msiPath + "\" /qn /norestart INSTALLFOLDER=\"" + folder + "\"",
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = true
            }))
            {
                if (process == null) return 1603;
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        private static int RunTestedUninstaller()
        {
            var path = ExtractEmbedded(UninstallerResourceSuffix, ".exe");
            try
            {
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                }))
                {
                    if (process == null) return 1603;
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }
            finally
            {
                try { if (File.Exists(path)) File.Delete(path); } catch { }
            }
        }

        private sealed class InstalledOwnProduct
        {
            public string DisplayVersion { get; set; }
            public string InstallLocation { get; set; }
        }

        private sealed class RequirementStatus
        {
            public bool Windows64 { get; set; }
            public bool Word { get; set; }
            public bool Excel { get; set; }
            public bool Office { get { return Word || Excel; } }
        }

        private sealed class InstallerWizard : Form
        {
            private readonly InstalledOwnProduct _own;
            private readonly RequirementStatus _requirements;
            private readonly Panel _content = new Panel();
            private readonly Label _title = new Label();
            private readonly Button _back = new Button();
            private readonly Button _next = new Button();
            private readonly Button _cancel = new Button();
            private readonly CheckBox _license = new CheckBox();
            private readonly TextBox _folder = new TextBox();
            private readonly Label _status = new Label();
            private int _page;
            private bool _installing;
            private string _msiPath;

            public InstallerWizard(InstalledOwnProduct own, RequirementStatus requirements)
            {
                _own = own;
                _requirements = requirements;
                Text = ProductName + " — O‘rnatish";
                StartPosition = FormStartPosition.CenterScreen;
                ClientSize = new Size(720, 430);
                MinimumSize = new Size(720, 430);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                Icon = SystemIcons.Application;

                _title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                _title.Location = new Point(28, 22);
                _title.AutoSize = true;
                Controls.Add(_title);

                _content.Location = new Point(28, 70);
                _content.Size = new Size(664, 285);
                Controls.Add(_content);

                _back.Text = "< Назад";
                _back.Location = new Point(360, 375);
                _back.Size = new Size(100, 32);
                _back.Click += delegate { if (_page > 0 && !_installing) { _page--; Render(); } };
                Controls.Add(_back);

                _next.Text = "Далее >";
                _next.Location = new Point(470, 375);
                _next.Size = new Size(100, 32);
                _next.Click += NextClicked;
                Controls.Add(_next);

                _cancel.Text = "Отмена";
                _cancel.Location = new Point(580, 375);
                _cancel.Size = new Size(90, 32);
                _cancel.Click += delegate { if (!_installing) DialogResult = DialogResult.Cancel; };
                Controls.Add(_cancel);
                Render();
            }

            private void Render()
            {
                _content.Controls.Clear();
                _back.Enabled = _page > 0 && !_installing;
                _cancel.Enabled = !_installing;
                if (_page == 0) RenderRequirements();
                else if (_page == 1) RenderLicense();
                else if (_page == 2) RenderFolder();
                else if (_page == 3) RenderOldVersion();
                else if (_page == 4) RenderInstalling();
                else RenderDone();
            }

            private void RenderRequirements()
            {
                _title.Text = "Texnik va dasturiy shartlar";
                AddText("Tarjimon Office UZ Word va Excel uchun o‘rnatiladi.\r\n\r\n" +
                    "• Windows 64-bit: " + (_requirements.Windows64 ? "mavjud" : "talab qilinadi") + "\r\n" +
                    "• Microsoft Word: " + (_requirements.Word ? "aniqlandi" : "aniqlanmadi") + "\r\n" +
                    "• Microsoft Excel: " + (_requirements.Excel ? "aniqlandi" : "aniqlanmadi") + "\r\n\r\n" +
                    (_requirements.Office ? "Office muhiti aniqlandi." : "Diqqat: Word/Excel aniqlanmadi."));
                _next.Text = "Далее >";
                _next.Enabled = _requirements.Windows64;
            }

            private void RenderLicense()
            {
                _title.Text = "Texnik/dasturiy shartlarga rozilik";

                var terms = new RichTextBox
                {
                    Location = new Point(20, 5),
                    Size = new Size(620, 190),
                    ReadOnly = true,
                    DetectUrls = false,
                    ScrollBars = RichTextBoxScrollBars.Vertical,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 9.5f)
                };

                string rtf = ReadEmbeddedText(LicenseResourceSuffix);
                if (!string.IsNullOrWhiteSpace(rtf))
                    terms.Rtf = rtf;
                else
                    terms.Text = "Tarjimon Office UZ dasturidan foydalanish va o‘rnatish shartlari.";
                _content.Controls.Add(terms);

                _license.Text = "Roziman / qabul qilaman";
                _license.AutoSize = true;
                _license.Location = new Point(20, 215);
                _license.Checked = true;
                _license.CheckedChanged -= LicenseChanged;
                _license.CheckedChanged += LicenseChanged;
                _content.Controls.Add(_license);

                _next.Text = "Далее >";
                _next.Enabled = _license.Checked;
            }

            private void LicenseChanged(object sender, EventArgs e)
            {
                if (_page == 1 && !_installing)
                    _next.Enabled = _license.Checked;
            }

            private void RenderFolder()
            {
                _title.Text = "O‘rnatish papkasi";
                AddText("Dastur quyidagi papkaga o‘rnatiladi:");

                _folder.Location = new Point(20, 70);
                _folder.Size = new Size(520, 28);
                if (string.IsNullOrWhiteSpace(_folder.Text))
                    _folder.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), DefaultFolderName);
                _content.Controls.Add(_folder);

                var browse = new Button
                {
                    Text = "Обзор...",
                    Location = new Point(550, 68),
                    Size = new Size(90, 30)
                };
                browse.Click += delegate
                {
                    using (var dialog = new FolderBrowserDialog
                    {
                        SelectedPath = Directory.Exists(_folder.Text) ? _folder.Text : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        Description = "Tarjimon Office UZ uchun o‘rnatish papkasini tanlang."
                    })
                    {
                        if (dialog.ShowDialog(this) == DialogResult.OK)
                            _folder.Text = dialog.SelectedPath;
                    }
                };
                _content.Controls.Add(browse);

                AddTextAt("Standart papka: " + _folder.Text + "\r\n\r\nKerak bo‘lsa, «Обзор...» orqali boshqa papkani tanlang.", 20, 120, 620, 100);
                _next.Text = "Далее >";
                _next.Enabled = true;
            }

            private void RenderOldVersion()
            {
                if (_own == null)
                {
                    _title.Text = "Eski versiyani tekshirish";
                    AddText("Tarjimon Office UZ ning eski versiyasi topilmadi.\r\n\r\n" +
                        "O‘rnatish davom ettiriladi va Windows Installer yangi versiyani o‘rnatadi.");
                }
                else
                {
                    _title.Text = "Eski versiyani o‘chirish";
                    AddText("Tarjimon Office UZ allaqachon o‘rnatilgan.\r\n\r\n" +
                        "O‘rnatilgan versiya: " + (_own.DisplayVersion ?? "aniqlanmadi") + "\r\n\r\n" +
                        "«Установить» tugmasini bossangiz, eski versiya Windows Installer orqali avtomatik o‘chiriladi va tugashi bilan yangi versiya o‘rnatiladi.\r\n\r\n" +
                        "Agar davom etishni istamasangiz, «Отмена» tugmasini bosing.");
                }
                _next.Text = "Установить";
                _next.Enabled = true;
            }

            private void RenderInstalling()
            {
                _title.Text = "O‘rnatilmoqda";
                AddText("Iltimos, kuting. Tarjimon Office UZ o‘rnatilmoqda...\r\n\r\nWord va Excel komponentlari o‘rnatiladi.\r\n\r\nEski versiya mavjud bo‘lsa, u Windows Installer orqali avtomatik o‘chiriladi.\r\n\r\nBu oynani yopmang.");
                _status.Text = _own == null ? "Windows Installer ishlamoqda..." : "Eski versiya o‘chirilmoqda, keyin yangi versiya o‘rnatiladi...";
                _status.Location = new Point(20, 220);
                _status.AutoSize = true;
                _content.Controls.Add(_status);
                _next.Enabled = false;
                _back.Enabled = false;
            }

            private void RenderDone()
            {
                _title.Text = "O‘rnatish tugadi";
                AddText("Tarjimon Office UZ muvaffaqiyatli o‘rnatildi.");
                _next.Text = "OK";
                _next.Enabled = true;
                _back.Enabled = false;
                _cancel.Enabled = false;
            }

            private void AddText(string text)
            {
                AddTextAt(text, 20, 10, 620, 190);
            }

            private void AddTextAt(string text, int x, int y, int width, int height)
            {
                _content.Controls.Add(new Label
                {
                    Text = text,
                    Location = new Point(x, y),
                    Size = new Size(width, height),
                    Font = new Font("Segoe UI", 10),
                    AutoSize = false
                });
            }

            private void NextClicked(object sender, EventArgs e)
            {
                if (_page == 0)
                {
                    if (!_requirements.Windows64)
                    {
                        MessageBox.Show("Ushbu installer 64-bit Windows uchun mo‘ljallangan.", ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    _page = 1;
                    Render();
                    return;
                }

                if (_page == 1)
                {
                    if (!_license.Checked)
                    {
                        MessageBox.Show("Davom etish uchun «Roziman / qabul qilaman» belgisini qo‘ying.", ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    _page = 2;
                    Render();
                    return;
                }

                if (_page == 2)
                {
                    if (string.IsNullOrWhiteSpace(_folder.Text))
                    {
                        MessageBox.Show("O‘rnatish papkasini ko‘rsating.", ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    _page = 3;
                    Render();
                    return;
                }

                if (_page == 3)
                {
                    if (_own != null)
                    {
                        var answer = MessageBox.Show(
                            "Eski Tarjimon Office UZ versiyasi topildi. Uni o‘chirib, yangi versiyani o‘rnatishga rozimisiz?",
                            ProductName,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (answer != DialogResult.Yes)
                        {
                            DialogResult = DialogResult.Cancel;
                            return;
                        }
                    }
                    StartInstall();
                    return;
                }

                if (_page == 5)
                    DialogResult = DialogResult.OK;
            }

            private void StartInstall()
            {
                _installing = true;
                _page = 4;
                Render();
                try
                {
                    _msiPath = ExtractEmbedded("TarjimonOfficeUZSetup.msi", ".msi");
                    var folder = _folder.Text.Trim().TrimEnd('\\');
                    Task.Run(delegate
                    {
                        int exitCode = 1603;
                        try
                        {
                            if (_own != null)
                            {
                                var uninstallCode = RunTestedUninstaller();
                                if (uninstallCode != 0)
                                {
                                    BeginInvoke((Action)(delegate { FinishInstall(uninstallCode); }));
                                    return;
                                }
                            }
                            exitCode = RunMsiInstall(_msiPath, folder);
                        }
                        catch
                        {
                        }
                        BeginInvoke((Action)(delegate { FinishInstall(exitCode); }));
                    });
                }
                catch (Exception ex)
                {
                    _installing = false;
                    MessageBox.Show(ex.ToString(), ProductName + " — Installer xatosi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _page = 3;
                    Render();
                }
            }

            private void FinishInstall(int exitCode)
            {
                try { if (!string.IsNullOrWhiteSpace(_msiPath) && File.Exists(_msiPath)) File.Delete(_msiPath); } catch { }
                _installing = false;
                if (exitCode == 0 || exitCode == 3010 || exitCode == 1641)
                {
                    _page = 5;
                    Render();
                    return;
                }
                MessageBox.Show("Windows Installer yoki Uninstaller xatosi. Kod: " + exitCode, ProductName + " — Installer xatosi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _page = 3;
                Render();
            }
        }
    }
}

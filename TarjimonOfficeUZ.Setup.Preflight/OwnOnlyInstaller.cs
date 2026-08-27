using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TarjimonOfficeUZ.Setup.Preflight
{
    internal static class OwnOnlyInstaller
    {
        private const string OwnDisplayName = "Tarjimon Office UZ";
        private const string OwnSetupDisplayName = "TarjimonOfficeUZ.Setup";
        private const string OwnPublisher = "Dostonjon Ashurov";

        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            try
            {
                var own = FindOwnProduct();
                if (own != null)
                {
                    var location = string.IsNullOrWhiteSpace(own.InstallLocation)
                        ? "Aniqlanmadi"
                        : own.InstallLocation;
                    var version = string.IsNullOrWhiteSpace(own.DisplayVersion)
                        ? "Aniqlanmadi"
                        : own.DisplayVersion;

                    var message =
                        "Tarjimon Office UZ allaqachon o'rnatilgan.\n\n" +
                        "O'rnatilgan versiya: " + version + "\n" +
                        "O'rnatilgan joy: " + location + "\n\n" +
                        "Eski versiyani olib tashlab, yangi versiyani o'rnatishga rozimisiz?";

                    var answer = MessageBox.Show(
                        message,
                        OwnDisplayName + " — Yangilash",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);

                    if (answer != DialogResult.Yes)
                        return 0;

                    if (!TryUninstall(own))
                    {
                        MessageBox.Show(
                            "Eski Tarjimon Office UZ versiyasini o'chirish amalga oshmadi. Yangi versiya o'rnatilmadi.",
                            OwnDisplayName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return 1603;
                    }
                }

                var msi = ExtractMsi();
                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = "/i \"" + msi + "\"",
                    UseShellExecute = true,
                    Verb = "runas"
                }))
                {
                    if (process == null) return 1603;
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    OwnDisplayName + " — Installer xatosi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                    var publisher = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                    var uninstallString = Convert.ToString(key.GetValue("QuietUninstallString")) ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(uninstallString))
                        uninstallString = Convert.ToString(key.GetValue("UninstallString")) ?? string.Empty;

                    if (!IsOwnProduct(displayName, publisher, keyName)) continue;

                    return new InstalledOwnProduct
                    {
                        DisplayName = displayName,
                        DisplayVersion = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty,
                        InstallLocation = Convert.ToString(key.GetValue("InstallLocation")) ?? string.Empty,
                        UninstallString = uninstallString,
                        RegistryHive = hive,
                        RegistryView = view,
                        KeyName = keyName
                    };
                }
            }

            return null;
        }

        private static bool IsOwnProduct(string displayName, string publisher, string keyName)
        {
            var name = (displayName ?? string.Empty).Trim();
            if (name.Equals(OwnDisplayName, StringComparison.OrdinalIgnoreCase)) return true;
            if (name.Equals(OwnSetupDisplayName, StringComparison.OrdinalIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(publisher) &&
                publisher.Equals(OwnPublisher, StringComparison.OrdinalIgnoreCase) &&
                (name.IndexOf("TarjimonOfficeUZ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 name.IndexOf("Tarjimon Office UZ", StringComparison.OrdinalIgnoreCase) >= 0))
                return true;

            return keyName.Equals("{EF08E22E-AFAD-45D2-BB8F-4099846EDB5E}", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryUninstall(InstalledOwnProduct product)
        {
            if (string.IsNullOrWhiteSpace(product.UninstallString)) return false;

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + product.UninstallString,
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = false
            };

            using (var process = Process.Start(psi))
            {
                if (process == null) return false;
                process.WaitForExit();
                return process.ExitCode == 0;
            }
        }

        private static string ExtractMsi()
        {
            var resource = typeof(OwnOnlyInstaller).Assembly
                .GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith("TarjimonOfficeUZSetup.msi", StringComparison.OrdinalIgnoreCase));

            if (resource == null)
                throw new FileNotFoundException("Embedded MSI topilmadi.");

            var directory = Path.Combine(Path.GetTempPath(), "TarjimonOfficeUZ");
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, Guid.NewGuid().ToString("N") + ".msi");

            using (var input = typeof(OwnOnlyInstaller).Assembly.GetManifestResourceStream(resource))
            {
                if (input == null) throw new FileNotFoundException("Embedded MSI topilmadi.");
                using (var output = File.Create(path)) input.CopyTo(output);
            }

            return path;
        }

        private sealed class InstalledOwnProduct
        {
            public string DisplayName { get; set; }
            public string DisplayVersion { get; set; }
            public string InstallLocation { get; set; }
            public string UninstallString { get; set; }
            public RegistryHive RegistryHive { get; set; }
            public RegistryView RegistryView { get; set; }
            public string KeyName { get; set; }
        }
    }
}

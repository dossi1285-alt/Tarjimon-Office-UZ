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
        private const string OwnPublisher = "Dostonjon Ashurov";

        [STAThread]
        private static int Main()
        {
            Application.EnableVisualStyles();
            try
            {
                // Do not ask about uninstall here. The user must first go through
                // the MSI wizard (license, installation folder and final confirmation).
                // If an older product exists, pass a public MSI property so the
                // custom final upgrade dialog can ask for removal confirmation there.
                var own = FindOwnProduct();
                var msi = ExtractMsi();
                var arguments = "/i \"" + msi + "\"";

                if (own != null)
                    arguments += " TARJIMON_UPGRADE=1";

                arguments += " /qf";

                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "msiexec.exe",
                    Arguments = arguments,
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
                    if (!displayName.Equals(OwnDisplayName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var publisher = Convert.ToString(key.GetValue("Publisher")) ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(publisher) &&
                        !publisher.Equals(OwnPublisher, StringComparison.OrdinalIgnoreCase))
                        continue;

                    return new InstalledOwnProduct
                    {
                        DisplayVersion = Convert.ToString(key.GetValue("DisplayVersion")) ?? string.Empty,
                        InstallLocation = Convert.ToString(key.GetValue("InstallLocation")) ?? string.Empty
                    };
                }
            }

            return null;
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
                using (var output = File.Create(path))
                    input.CopyTo(output);
            }

            return path;
        }

        private sealed class InstalledOwnProduct
        {
            public string DisplayVersion { get; set; }
            public string InstallLocation { get; set; }
        }
    }
}

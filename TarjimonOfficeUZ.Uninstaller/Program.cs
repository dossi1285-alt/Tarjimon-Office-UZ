using Microsoft.Win32;
using System.Diagnostics;

namespace TarjimonOfficeUZ.Uninstaller;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        const string displayName = "Tarjimon Office UZ";
        string? productCode = FindProductCode(displayName);

        if (string.IsNullOrWhiteSpace(productCode))
        {
            MessageBox.Show(
                "Tarjimon Office UZ topilmadi.",
                "Tarjimon Office UZ — O‘chirish",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = $"/x {productCode}",
                UseShellExecute = true,
                Verb = "runas"
            });
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // User cancelled UAC or Windows Installer could not be started.
        }
    }

    private static string? FindProductCode(string displayName)
    {
        string[] uninstallRoots =
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (RegistryHive hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        {
            foreach (string root in uninstallRoots)
            {
                using RegistryKey? baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(root);
                string? result = FindInKey(baseKey, displayName);
                if (result != null) return result;

                using RegistryKey? baseKey32 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32).OpenSubKey(root);
                result = FindInKey(baseKey32, displayName);
                if (result != null) return result;
            }
        }

        return null;
    }

    private static string? FindInKey(RegistryKey? uninstallKey, string displayName)
    {
        if (uninstallKey == null) return null;

        foreach (string subKeyName in uninstallKey.GetSubKeyNames())
        {
            using RegistryKey? subKey = uninstallKey.OpenSubKey(subKeyName);
            string? name = subKey?.GetValue("DisplayName") as string;
            string? productCode = subKey?.GetValue("ProductCode") as string;

            if (string.Equals(name, displayName, StringComparison.OrdinalIgnoreCase))
                return productCode ?? (subKeyName.StartsWith("{", StringComparison.Ordinal) ? subKeyName : null);
        }

        return null;
    }
}

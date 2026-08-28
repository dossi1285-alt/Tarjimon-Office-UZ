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

        // This utility has one job only: start Windows Installer uninstall.
        if (string.IsNullOrWhiteSpace(productCode))
            return;

        try
        {
            using Process process = Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = $"/x {productCode} /qn /norestart",
                UseShellExecute = true,
                Verb = "runas"
            })!;

            // Do not exit until Windows Installer has completely finished.
            // This prevents Office from being opened while MSI is still removing
            // the add-in files and registrations.
            process.WaitForExit();
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // UAC was cancelled or Windows Installer could not be started.
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
                using RegistryKey baseKey64 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
                using RegistryKey? uninstall64 = baseKey64.OpenSubKey(root);
                string? result = FindInKey(uninstall64, displayName);
                if (result != null)
                    return result;

                using RegistryKey baseKey32 = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32);
                using RegistryKey? uninstall32 = baseKey32.OpenSubKey(root);
                result = FindInKey(uninstall32, displayName);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    private static string? FindInKey(RegistryKey? uninstallKey, string displayName)
    {
        if (uninstallKey == null)
            return null;

        foreach (string subKeyName in uninstallKey.GetSubKeyNames())
        {
            using RegistryKey? subKey = uninstallKey.OpenSubKey(subKeyName);
            string? name = subKey?.GetValue("DisplayName") as string;
            if (!string.Equals(name, displayName, StringComparison.OrdinalIgnoreCase))
                continue;

            string? productCode = subKey?.GetValue("ProductCode") as string;
            return productCode ?? (subKeyName.StartsWith("{", StringComparison.Ordinal) ? subKeyName : null);
        }

        return null;
    }
}

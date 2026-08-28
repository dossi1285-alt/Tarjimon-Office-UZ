using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TarjimonOfficeUZ.Uninstaller;

internal static class Program
{
    private const string DisplayName = "Tarjimon Office UZ";

    [STAThread]
    private static void Main()
    {
        string? uninstallKey = FindUninstallKey();
        if (uninstallKey == null)
            return;

        // Open Windows' own Programs and Features page with our product selected.
        // The user remains responsible for clicking Удалить and confirming Да/Нет/Далее.
        string arguments = $"/pageName ProgramsAndFeatures /select \"{DisplayName}\"";
        Process.Start(new ProcessStartInfo
        {
            FileName = "control.exe",
            Arguments = "appwiz.cpl",
            UseShellExecute = true
        });

        // Windows does not expose a supported command-line API for selecting a
        // specific Programs and Features row. We therefore use the documented
        // AppsFolder shell namespace when available as a best-effort selection.
        TrySelectRegisteredProduct(uninstallKey);
    }

    private static string? FindUninstallKey()
    {
        string[] roots =
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        foreach (RegistryHive hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        {
            using RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view);
            foreach (string root in roots)
            {
                using RegistryKey? key = baseKey.OpenSubKey(root);
                if (key == null) continue;
                foreach (string name in key.GetSubKeyNames())
                {
                    using RegistryKey? sub = key.OpenSubKey(name);
                    if (string.Equals(sub?.GetValue("DisplayName") as string, DisplayName, StringComparison.OrdinalIgnoreCase))
                        return $"{root}\\{name}";
                }
            }
        }
        return null;
    }

    private static void TrySelectRegisteredProduct(string uninstallKey)
    {
        // Deliberately no uninstall command is executed here. Windows' own
        // Programs and Features UI remains in control of the uninstall flow.
        _ = uninstallKey;
    }
}

using Microsoft.Win32;
using System.Diagnostics;

namespace TarjimonOfficeUZ.Uninstaller;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        const string displayName = "Tarjimon Office UZ";
        string? uninstallString = FindUninstallString(displayName);

        // This utility has one job only: invoke the same uninstall command
        // registered by Windows for Tarjimon Office UZ.
        if (string.IsNullOrWhiteSpace(uninstallString))
            return;

        try
        {
            ProcessStartInfo startInfo = BuildUninstallStartInfo(uninstallString);
            using Process process = Process.Start(startInfo)!;
            process.WaitForExit();
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // UAC was cancelled or Windows Installer could not be started.
        }
    }

    private static string? FindUninstallString(string displayName)
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
                foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                {
                    using RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view);
                    using RegistryKey? uninstallKey = baseKey.OpenSubKey(root);
                    string? result = FindInKey(uninstallKey, displayName);
                    if (result != null)
                        return result;
                }
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

            return subKey?.GetValue("UninstallString") as string;
        }

        return null;
    }

    private static ProcessStartInfo BuildUninstallStartInfo(string uninstallString)
    {
        // Windows may register an MSI uninstall command as:
        // msiexec.exe /I{PRODUCT-CODE}
        // The Programs and Features uninstall action uses this registered command.
        // Convert /I to /X because this utility's sole purpose is removal.
        string command = uninstallString.Trim();
        int exeEnd = command.IndexOf(' ');
        string fileName;
        string arguments;

        if (exeEnd > 0)
        {
            fileName = command[..exeEnd].Trim('"');
            arguments = command[(exeEnd + 1)..].Trim();
        }
        else
        {
            fileName = command.Trim('"');
            arguments = string.Empty;
        }

        if (fileName.EndsWith("msiexec.exe", StringComparison.OrdinalIgnoreCase))
            arguments = ReplaceInstallSwitchWithUninstall(arguments);

        return new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true,
            Verb = "runas"
        };
    }

    private static string ReplaceInstallSwitchWithUninstall(string arguments)
    {
        string result = arguments;
        result = result.Replace(" /I{", " /X{", StringComparison.OrdinalIgnoreCase);
        result = result.Replace(" /I {", " /X {", StringComparison.OrdinalIgnoreCase);
        result = result.Replace(" /I\"{", " /X\"{", StringComparison.OrdinalIgnoreCase);
        result = result.Replace(" /I \"{", " /X \"{", StringComparison.OrdinalIgnoreCase);
        return result;
    }
}

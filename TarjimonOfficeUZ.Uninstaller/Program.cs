using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace TarjimonOfficeUZ.Uninstaller;

internal static class Program
{
    private const string ProductName = "Tarjimon Office UZ";
    private const string ProductPublisher = "Dostonjon Ashurov";
    private const int WaitSeconds = 600;

    [STAThread]
    private static int Main()
    {
        try
        {
            var product = FindInstalledProduct();
            if (product == null)
                return 0;

            if (string.IsNullOrWhiteSpace(product.UninstallString))
                return 1603;

            int exitCode = RunWindowsUninstall(product.UninstallString);
            if (exitCode != 0 && exitCode != 3010 && exitCode != 1641)
                return exitCode;

            var deadline = DateTime.UtcNow.AddSeconds(WaitSeconds);
            while (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(500);
                if (FindInstalledProduct() == null)
                    return 0;
            }

            return 1602;
        }
        catch
        {
            return 1603;
        }
    }

    private static int RunWindowsUninstall(string uninstallString)
    {
        var command = ParseCommand(uninstallString);
        if (string.IsNullOrWhiteSpace(command.FileName))
            return 1603;

        string arguments = command.Arguments;
        bool isMsiExec = System.IO.Path.GetFileName(command.FileName)
            .Equals("msiexec.exe", StringComparison.OrdinalIgnoreCase);

        if (isMsiExec)
        {
            arguments = ConvertMsiInstallToUninstall(arguments);
            if (arguments.IndexOf("/quiet", StringComparison.OrdinalIgnoreCase) < 0 &&
                arguments.IndexOf("/qn", StringComparison.OrdinalIgnoreCase) < 0)
                arguments += " /qn";
            if (arguments.IndexOf("/norestart", StringComparison.OrdinalIgnoreCase) < 0)
                arguments += " /norestart";
        }

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = command.FileName,
            Arguments = arguments,
            UseShellExecute = true,
            Verb = "runas",
            CreateNoWindow = true
        });

        if (process == null)
            return 1603;

        process.WaitForExit();
        return process.ExitCode;
    }

    private static string ConvertMsiInstallToUninstall(string arguments)
    {
        var result = arguments;
        result = ReplaceSwitch(result, "/I", "/X");
        result = ReplaceSwitch(result, "/i", "/X");
        return result;
    }

    private static string ReplaceSwitch(string value, string from, string to)
    {
        int index = value.IndexOf(from, StringComparison.OrdinalIgnoreCase);
        while (index >= 0)
        {
            bool leftBoundary = index == 0 || char.IsWhiteSpace(value[index - 1]);
            bool rightBoundary = index + from.Length >= value.Length ||
                                 char.IsWhiteSpace(value[index + from.Length]) ||
                                 value[index + from.Length] == '{';
            if (leftBoundary && rightBoundary)
                return value.Substring(0, index) + to + value.Substring(index + from.Length);
            index = value.IndexOf(from, index + from.Length, StringComparison.OrdinalIgnoreCase);
        }
        return value;
    }

    private static (string FileName, string Arguments) ParseCommand(string commandLine)
    {
        string text = commandLine.Trim();
        if (text.Length == 0) return (string.Empty, string.Empty);

        if (text[0] == '"')
        {
            int end = text.IndexOf('"', 1);
            if (end < 0) return (text.Trim('"'), string.Empty);
            return (text.Substring(1, end - 1), text[(end + 1)..].Trim());
        }

        int space = text.IndexOf(' ');
        if (space < 0) return (text, string.Empty);
        return (text[..space], text[(space + 1)..].Trim());
    }

    private sealed class InstalledProduct
    {
        public string UninstallString { get; init; } = string.Empty;
    }

    private static InstalledProduct? FindInstalledProduct()
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
                if (!string.IsNullOrWhiteSpace(publisher) &&
                    !publisher.Equals(ProductPublisher, StringComparison.OrdinalIgnoreCase)) continue;

                var uninstallString = Convert.ToString(key.GetValue("QuietUninstallString")) ??
                                      Convert.ToString(key.GetValue("UninstallString")) ?? string.Empty;
                return new InstalledProduct { UninstallString = uninstallString };
            }
        }

        return null;
    }
}

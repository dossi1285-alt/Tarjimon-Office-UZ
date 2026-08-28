using System.Diagnostics;
using System.Windows.Automation;
using Microsoft.Win32;

namespace TarjimonOfficeUZ.Uninstaller;

internal static class Program
{
    private const string ProductName = "Tarjimon Office UZ";
    private const int WaitSeconds = 600;

    [STAThread]
    private static int Main()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "control.exe",
                Arguments = "appwiz.cpl",
                UseShellExecute = true
            });

            bool started = false;
            for (int i = 0; i < 60; i++)
            {
                Thread.Sleep(500);
                if (TrySelectAndStartUninstall())
                {
                    started = true;
                    break;
                }
            }

            if (!started)
                return 1602;

            if (!IsProductInstalled())
                return 0;

            // Windows'ning o'z uninstall oynasi foydalanuvchi tomonidan
            // boshqariladi. Bu EXE uninstallni o'zi bajarmaydi; faqat
            // Windows'ga boshlab beradi va tugashini kutadi.
            var deadline = DateTime.UtcNow.AddSeconds(WaitSeconds);
            while (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(1000);
                if (!IsProductInstalled())
                    return 0;
            }

            // Eski versiya hali Windows'da ro'yxatdan o'tgan bo'lsa,
            // Setup yangi MSI'ni ishga tushirmasligi kerak.
            return 1602;
        }
        catch
        {
            return 1603;
        }
    }

    private static bool TrySelectAndStartUninstall()
    {
        AutomationElement? window = AutomationElement.RootElement.FindFirst(
            TreeScope.Children,
            new OrCondition(
                new PropertyCondition(AutomationElement.ClassNameProperty, "CabinetWClass"),
                new PropertyCondition(AutomationElement.NameProperty, "Программы и компоненты"),
                new PropertyCondition(AutomationElement.NameProperty, "Programs and Features")));

        if (window == null) return false;

        AutomationElement? item = FindProductItem(window);
        if (item == null) return false;
        if (!item.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object selectionObject)) return false;

        ((SelectionItemPattern)selectionObject).Select();
        Thread.Sleep(300);

        AutomationElement? uninstall = FindUninstallControl(window);
        if (uninstall == null) return false;
        if (!uninstall.TryGetCurrentPattern(InvokePattern.Pattern, out object invokeObject)) return false;

        ((InvokePattern)invokeObject).Invoke();
        return true;
    }

    private static AutomationElement? FindProductItem(AutomationElement window)
    {
        AutomationElementCollection elements = window.FindAll(
            TreeScope.Descendants,
            new OrCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem)));

        foreach (AutomationElement element in elements)
        {
            if ((element.Current.Name ?? string.Empty).Contains(ProductName, StringComparison.OrdinalIgnoreCase))
                return element;
        }
        return null;
    }

    private static AutomationElement? FindUninstallControl(AutomationElement window)
    {
        AutomationElementCollection controls = window.FindAll(
            TreeScope.Descendants,
            new OrCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)));

        foreach (AutomationElement control in controls)
        {
            string name = control.Current.Name ?? string.Empty;
            if (name.Contains("Удалить", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Uninstall", StringComparison.OrdinalIgnoreCase))
                return control;
        }
        return null;
    }

    private static bool IsProductInstalled()
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
                if (displayName.Equals(ProductName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        return false;
    }
}

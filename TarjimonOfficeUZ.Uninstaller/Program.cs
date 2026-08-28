using System.Diagnostics;
using System.Windows.Automation;

namespace TarjimonOfficeUZ.Uninstaller;

internal static class Program
{
    private const string ProductName = "Tarjimon Office UZ";

    [STAThread]
    private static void Main()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "control.exe",
            Arguments = "appwiz.cpl",
            UseShellExecute = true
        });

        // Wait for Programs and Features, select our product, then invoke
        // Windows' own Удалить command. All later confirmations remain with the user.
        for (int i = 0; i < 30; i++)
        {
            Thread.Sleep(500);
            if (TrySelectAndStartUninstall())
                return;
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

        if (window == null)
            return false;

        AutomationElement? item = FindProductItem(window);
        if (item == null)
            return false;

        if (!item.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object selectionObject))
            return false;

        ((SelectionItemPattern)selectionObject).Select();
        Thread.Sleep(300);

        AutomationElement? uninstall = FindUninstallControl(window);
        if (uninstall == null)
            return false;

        if (!uninstall.TryGetCurrentPattern(InvokePattern.Pattern, out object invokeObject))
            return false;

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
            string name = element.Current.Name ?? string.Empty;
            if (name.Contains(ProductName, StringComparison.OrdinalIgnoreCase))
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
}

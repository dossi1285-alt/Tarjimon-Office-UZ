using Microsoft.Win32;

namespace TarjimonOfficeUZ.Shared.Services
{
    public static class OfficeAddInStartupService
    {
        private const string WordAddInKey =
            @"Software\Microsoft\Office\Word\Addins\TarjimonOfficeUZ.Word";

        private const string ExcelAddInKey =
            @"Software\Microsoft\Office\Excel\Addins\TarjimonOfficeUZ.Excel";

        private const int LoadAtStartup = 3;
        private const int DoNotLoadAtStartup = 0;

        public static bool SetWordStartup(bool enabled)
        {
            return SetLoadBehavior(WordAddInKey, enabled);
        }

        public static bool SetExcelStartup(bool enabled)
        {
            return SetLoadBehavior(ExcelAddInKey, enabled);
        }

        private static bool SetLoadBehavior(string addInKey, bool enabled)
        {
            try
            {
                using (RegistryKey key =
                    Registry.CurrentUser.OpenSubKey(addInKey, writable: true))
                {
                    if (key == null)
                        return false;

                    key.SetValue(
                        "LoadBehavior",
                        enabled ? LoadAtStartup : DoNotLoadAtStartup,
                        RegistryValueKind.DWord);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

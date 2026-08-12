namespace TarjimonOfficeUZ.Shared.Managers
{
    public class SettingsModel
    {
        public string Language { get; set; } = "uz";

        public bool AutoCheckUpdates { get; set; } = true;

        public bool StartWithWord { get; set; }

        public bool StartWithExcel { get; set; }

        public bool EnableFutureAlphabet { get; set; } = false;
    }
}

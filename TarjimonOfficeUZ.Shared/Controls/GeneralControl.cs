using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Managers;
using TarjimonOfficeUZ.Shared.Services;

namespace TarjimonOfficeUZ.Shared.Controls
{
    public partial class GeneralControl : UserControl
    {
        public GeneralControl()
        {
            InitializeComponent();
            LoadSettings();
            ApplyLanguage();
        }

        private void LoadSettings()
        {
            chkWordStartup.Checked = SettingsManager.Current.StartWithWord;
            chkExcelStartup.Checked = SettingsManager.Current.StartWithExcel;
            chkAutoCheckUpdates.Checked = SettingsManager.Current.AutoCheckUpdates;
        }

        public void ApplyLanguage()
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru":
                    lblStartup.Text = "Запуск Office";
                    chkWordStartup.Text = "Запускать с Microsoft Word";
                    chkExcelStartup.Text = "Запускать с Microsoft Excel";
                    chkAutoCheckUpdates.Text = "Автоматически проверять обновления";
                    btnSave.Text = "Сохранить";
                    btnCancel.Text = "Отмена";
                    break;

                case "en":
                    lblStartup.Text = "Office startup";
                    chkWordStartup.Text = "Start with Microsoft Word";
                    chkExcelStartup.Text = "Start with Microsoft Excel";
                    chkAutoCheckUpdates.Text = "Automatically check for updates";
                    btnSave.Text = "Save";
                    btnCancel.Text = "Cancel";
                    break;

                default:
                    lblStartup.Text = "Office ishga tushishi";
                    chkWordStartup.Text = "Microsoft Word bilan ishga tushirish";
                    chkExcelStartup.Text = "Microsoft Excel bilan ishga tushirish";
                    chkAutoCheckUpdates.Text = "Yangilanishlarni avtomatik tekshirish";
                    btnSave.Text = "Saqlash";
                    btnCancel.Text = "Bekor qilish";
                    break;
            }
        }

        private void GeneralControl_Load(object sender, EventArgs e)
        {
            LoadSettings();
            ApplyLanguage();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool wordApplied = OfficeAddInStartupService.SetWordStartup(chkWordStartup.Checked);
            bool excelApplied = OfficeAddInStartupService.SetExcelStartup(chkExcelStartup.Checked);

            SettingsManager.Current.StartWithWord = chkWordStartup.Checked;
            SettingsManager.Current.StartWithExcel = chkExcelStartup.Checked;
            SettingsManager.Current.AutoCheckUpdates = chkAutoCheckUpdates.Checked;
            SettingsManager.Save();

            bool applied = wordApplied && excelApplied;
            string language = SettingsManager.Current.Language;

            if (applied)
            {
                MessageBox.Show(
                    language == "ru"
                        ? "Настройки сохранены. Изменения запуска Office применятся при следующем запуске Word или Excel."
                        : language == "en"
                            ? "Settings saved. Office startup changes will apply the next time Word or Excel starts."
                            : "Sozlamalar saqlandi. Office ishga tushirish o‘zgarishlari Word yoki Excel keyingi ishga tushganda qo‘llanadi.",
                    "Tarjimon Office UZ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(
                language == "ru"
                    ? "Настройки сохранены, но один или оба параметра запуска Office не удалось применить."
                    : language == "en"
                        ? "Settings were saved, but one or both Office startup settings could not be applied."
                        : "Sozlamalar saqlandi, lekin Word yoki Excel ishga tushirish sozlamalaridan biri qo‘llanmadi.",
                "Tarjimon Office UZ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }
    }
}
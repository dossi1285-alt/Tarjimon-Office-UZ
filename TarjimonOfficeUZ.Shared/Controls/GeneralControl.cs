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

            chkWordStartup.Checked =
                SettingsManager.Current.StartWithWord;

            chkExcelStartup.Checked =
                SettingsManager.Current.StartWithExcel;
        }

        private void GeneralControl_Load(object sender, EventArgs e)
        {
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool wordApplied = OfficeAddInStartupService.SetWordStartup(
                chkWordStartup.Checked);

            bool excelApplied = OfficeAddInStartupService.SetExcelStartup(
                chkExcelStartup.Checked);

            SettingsManager.Current.StartWithWord =
                chkWordStartup.Checked;

            SettingsManager.Current.StartWithExcel =
                chkExcelStartup.Checked;

            SettingsManager.Save();

            if (wordApplied && excelApplied)
            {
                MessageBox.Show(
                    "Settings saved successfully. Changes will apply the next time Word or Excel starts.",
                    "Tarjimon Office UZ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            MessageBox.Show(
                "Settings were saved, but one or both Office add-in startup settings could not be applied. Please restart Word/Excel and check the add-in registration.",
                "Tarjimon Office UZ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            chkWordStartup.Checked =
                SettingsManager.Current.StartWithWord;

            chkExcelStartup.Checked =
                SettingsManager.Current.StartWithExcel;
        }
    }
}
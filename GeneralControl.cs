using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Managers;

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
            SettingsManager.Current.StartWithWord =
                chkWordStartup.Checked;

            SettingsManager.Current.StartWithExcel =
                chkExcelStartup.Checked;

            SettingsManager.Save();

            MessageBox.Show(
                "Settings saved successfully.",
                "Tarjimon Office UZ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
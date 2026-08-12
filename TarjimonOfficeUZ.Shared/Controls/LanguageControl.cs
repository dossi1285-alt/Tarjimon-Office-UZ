using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Managers;

namespace TarjimonOfficeUZ.Shared.Controls
{
    public partial class LanguageControl : UserControl
    {
        public LanguageControl()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            cmbLanguage.Items.Clear();

            cmbLanguage.Items.Add("O'zbek");
            cmbLanguage.Items.Add("Русский");
            cmbLanguage.Items.Add("English");

            switch (SettingsManager.Current.Language)
            {
                case "uz":
                    cmbLanguage.SelectedIndex = 0;
                    break;

                case "ru":
                    cmbLanguage.SelectedIndex = 1;
                    break;

                case "en":
                    cmbLanguage.SelectedIndex = 2;
                    break;

                default:
                    cmbLanguage.SelectedIndex = 0;
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            switch (cmbLanguage.SelectedIndex)
            {
                case 0:
                    SettingsManager.Current.Language = "uz";
                    break;

                case 1:
                    SettingsManager.Current.Language = "ru";
                    break;

                case 2:
                    SettingsManager.Current.Language = "en";
                    break;
            }

            SettingsManager.Save();

            MessageBox.Show(
                "Language saved successfully.",
                "Tarjimon Office UZ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            switch (SettingsManager.Current.Language)
            {
                case "uz":
                    cmbLanguage.SelectedIndex = 0;
                    break;

                case "ru":
                    cmbLanguage.SelectedIndex = 1;
                    break;

                case "en":
                    cmbLanguage.SelectedIndex = 2;
                    break;
            }
        }

        
    }
}
using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Managers;
using TarjimonOfficeUZ.Shared.Forms;

namespace TarjimonOfficeUZ.Shared.Controls
{
    public partial class LanguageControl : UserControl
    {
        public LanguageControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            cmbLanguage.Items.Clear();
            cmbLanguage.Items.Add("O‘zbek");
            cmbLanguage.Items.Add("Русский");
            cmbLanguage.Items.Add("English");
            SelectCurrentLanguage();
            ApplyLanguage();
        }

        private void SelectCurrentLanguage()
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru": cmbLanguage.SelectedIndex = 1; break;
                case "en": cmbLanguage.SelectedIndex = 2; break;
                default: cmbLanguage.SelectedIndex = 0; break;
            }
        }

        public void ApplyLanguage()
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru":
                    lblLanguage.Text = "Язык интерфейса";
                    btnSave.Text = "Сохранить";
                    btnCancel.Text = "Отмена";
                    break;
                case "en":
                    lblLanguage.Text = "Interface language";
                    btnSave.Text = "Save";
                    btnCancel.Text = "Cancel";
                    break;
                default:
                    lblLanguage.Text = "Interfeys tili";
                    btnSave.Text = "Saqlash";
                    btnCancel.Text = "Bekor qilish";
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            switch (cmbLanguage.SelectedIndex)
            {
                case 1: SettingsManager.Current.Language = "ru"; break;
                case 2: SettingsManager.Current.Language = "en"; break;
                default: SettingsManager.Current.Language = "uz"; break;
            }

            SettingsManager.Save();
            ApplyLanguage();

            SettingsForm form = FindForm() as SettingsForm;
            if (form != null)
                form.ApplyLanguage();

            string language = SettingsManager.Current.Language;
            MessageBox.Show(
                language == "ru" ? "Язык сохранён." : language == "en" ? "Language saved." : "Til saqlandi.",
                "Tarjimon Office UZ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectCurrentLanguage();
        }
    }
}
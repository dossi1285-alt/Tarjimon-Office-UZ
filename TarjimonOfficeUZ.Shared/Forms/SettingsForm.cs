using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Controls;
using TarjimonOfficeUZ.Shared.Managers;

namespace TarjimonOfficeUZ.Shared.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly GeneralControl generalControl = new GeneralControl();
        private readonly LanguageControl languageControl = new LanguageControl();
        private readonly UpdateControl updateControl = new UpdateControl();
        private readonly AboutControl aboutControl = new AboutControl();

        public SettingsForm()
        {
            InitializeComponent();

            panelContent.Controls.Add(generalControl);
            panelContent.Controls.Add(languageControl);
            panelContent.Controls.Add(updateControl);
            panelContent.Controls.Add(aboutControl);

            generalControl.Dock = DockStyle.Fill;
            languageControl.Dock = DockStyle.Fill;
            updateControl.Dock = DockStyle.Fill;
            aboutControl.Dock = DockStyle.Fill;

            ApplyLanguage();
            generalControl.BringToFront();
        }

        public void ApplyLanguage()
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru":
                    Text = "Tarjimon Office UZ — Настройки";
                    btnGeneral.Text = "Общие";
                    btnLanguage.Text = "Язык";
                    btnUpdate.Text = "Обновление";
                    btnAbout.Text = "О программе";
                    break;
                case "en":
                    Text = "Tarjimon Office UZ Settings";
                    btnGeneral.Text = "General";
                    btnLanguage.Text = "Language";
                    btnUpdate.Text = "Update";
                    btnAbout.Text = "About";
                    break;
                default:
                    Text = "Tarjimon Office UZ — Sozlamalar";
                    btnGeneral.Text = "Umumiy";
                    btnLanguage.Text = "Til";
                    btnUpdate.Text = "Yangilanish";
                    btnAbout.Text = "Dastur haqida";
                    break;
            }

            generalControl.ApplyLanguage();
            languageControl.ApplyLanguage();
            updateControl.ApplyLanguage();
            aboutControl.ApplyLanguage();
        }

        private void btnGeneral_Click(object sender, EventArgs e)
        {
            generalControl.BringToFront();
        }

        private void btnLanguage_Click(object sender, EventArgs e)
        {
            languageControl.BringToFront();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateControl.BringToFront();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            aboutControl.BringToFront();
        }
    }
}
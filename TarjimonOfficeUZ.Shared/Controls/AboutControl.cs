using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Managers;

namespace TarjimonOfficeUZ.Shared.Controls
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            lblProduct.Text = Constants.Product;
            lblVersionValue.Text = Constants.Version;
            lblCopyright.Text = Constants.Copyright;
            lnkWebsite.Text = Constants.Website;
            lnkSupport.Text = Constants.SupportEmail;
            ApplyLanguage();
        }

        public void ApplyLanguage()
        {
            switch (SettingsManager.Current.Language)
            {
                case "ru":
                    lblVersion.Text = "Версия";
                    lblWebsite.Text = "Сайт проекта";
                    lblSupport.Text = "Поддержка";
                    btnClose.Text = "Закрыть";
                    break;
                case "en":
                    lblVersion.Text = "Version";
                    lblWebsite.Text = "Project website";
                    lblSupport.Text = "Support";
                    btnClose.Text = "Close";
                    break;
                default:
                    lblVersion.Text = "Versiya";
                    lblWebsite.Text = "Loyiha sayti";
                    lblSupport.Text = "Qo‘llab-quvvatlash";
                    btnClose.Text = "Yopish";
                    break;
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show(
                    SettingsManager.Current.Language == "ru"
                        ? "Ссылку не удалось открыть."
                        : SettingsManager.Current.Language == "en"
                            ? "The link could not be opened."
                            : "Havolani ochib bo‘lmadi.",
                    "Tarjimon Office UZ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void lnkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(Constants.Website);
        }

        private void lnkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(Constants.SupportEmail);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FindForm()?.Close();
        }
    }
}
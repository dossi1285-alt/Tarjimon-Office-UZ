using System.Windows.Forms;
using TarjimonOfficeUZ.Shared;

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
        }

        private void lnkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(lnkWebsite.Text);
            }
            catch
            {
                MessageBox.Show(
                    "Website could not be opened.",
                    "Tarjimon Office UZ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void lnkSupport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:" + lnkSupport.Text);
            }
            catch
            {
                MessageBox.Show(
                    "Email client could not be opened.",
                    "Tarjimon Office UZ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            FindForm()?.Close();

        }
    }
}
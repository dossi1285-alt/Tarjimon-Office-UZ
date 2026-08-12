using System.Windows.Forms;
using TarjimonOfficeUZ.Shared;

namespace TarjimonOfficeUZ.Shared.Controls
{
    public partial class UpdateControl : UserControl
    {
        public UpdateControl()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            lblVersion.Text = Constants.Version;
        }

        private void lblVersion_Click(object sender, System.EventArgs e)
        {

        }

        private void btnCheckUpdate_Click(object sender, System.EventArgs e)
        {

        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            FindForm()?.Close();

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TarjimonOfficeUZ.Shared.Controls;

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

            generalControl.BringToFront();
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

using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using TarjimonOfficeUZ.Shared;
using TarjimonOfficeUZ.Word;
using TarjimonOfficeUZ.Shared.Forms;

namespace TarjimonOfficeUZ.Word
{
    public partial class TarjimonRibbon
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            btnLatinToCyrillic.Image = ResourceLoader.A_A;
            btnCyrillicToLatin.Image = ResourceLoader.A_A;
            btnMenu.Image = ResourceLoader.Kalit;
        }

        private void btnLatinToCyrillic_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.ConvertSelectionLatinToCyrillic();
        }

        private void btnCyrillicToLatin_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.ConvertSelectionCyrillicToLatin();
        }

        private void btnMenu_Click(object sender, RibbonControlEventArgs e)
        {
            using (SettingsForm form = new SettingsForm())
            {
                form.ShowDialog();
            }
        }
    }
}

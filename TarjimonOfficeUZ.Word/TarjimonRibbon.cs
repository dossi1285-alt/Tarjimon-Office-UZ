using Microsoft.Office.Tools.Ribbon;
using TarjimonOfficeUZ.Shared;
using TarjimonOfficeUZ.Shared.Forms;

namespace TarjimonOfficeUZ.Word
{
    public partial class TarjimonRibbon
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            btnLatinToCyrillic.Image = ResourceLoader.LatinToCyrillic;
            btnCyrillicToLatin.Image = ResourceLoader.CyrillicToLatin;
            btnMenu.Image = ResourceLoader.Settings;
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

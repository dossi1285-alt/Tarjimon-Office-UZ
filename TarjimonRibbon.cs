using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Core.Translation;

namespace TarjimonOfficeUZ.Excel
{
    public partial class TarjimonRibbon
    {
        private void TarjimonRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private Worksheet GetActiveWorksheet()
        {
            return Globals.ThisAddIn.Application.ActiveSheet as Worksheet;
        }

        private Range GetSelectedRange()
        {
            return Globals.ThisAddIn.Application.Selection as Range;
        }
        private void ConvertSelectedCells(bool latinToCyrillic)
        {
            Range selectedRange = GetSelectedRange();

            if (selectedRange == null)
                return;

            Worksheet worksheet = GetActiveWorksheet();

            if (worksheet != null && worksheet.ProtectContents)
            {
                MessageBox.Show(
                    "Joriy varaq himoyalangan.\n\n" +
                    "Avval varaq himoyasini olib tashlang.",
                    "ADX Office",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

           

            foreach (Range cell in selectedRange.Cells)
            {
                try
                {
                    if (cell.HasFormula)
                        continue;

                    object value = cell.Value2;



                    if (value == null)
                        continue;

                    string text = Convert.ToString(value);

                    if (text == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(text))
                        continue;

                    string result;

                    if (latinToCyrillic)
                        result = Transliterator.LatinToCyrillic(text);
                    else
                        result = Transliterator.CyrillicToLatin(text);

                    if (!string.Equals(result, text, StringComparison.Ordinal))
                    {
                        cell.Value2 = result;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        private void btnCyrillicToLatin_Click(object sender, RibbonControlEventArgs e)
        {
            ConvertSelectedCells(false);
        }

        private void btnLatinToCyrillic_Click(object sender, RibbonControlEventArgs e)
        {
            ConvertSelectedCells(true);
        }

        private void btnADX_Click(object sender, RibbonControlEventArgs e)
        {
            MessageBox.Show(
                "ADX Office\n\n" +
                "Version: 1.0\n" +
                "© 2026 ADX\n\n" +
                "Tarjimon Office UZ",
                "ADX Office",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
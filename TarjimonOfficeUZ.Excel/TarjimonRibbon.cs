using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Windows.Forms;
using TarjimonOfficeUZ.Core.Translation;
using TarjimonOfficeUZ.Shared;
using TarjimonOfficeUZ.Shared.Forms;

namespace TarjimonOfficeUZ.Excel
{
    public partial class TarjimonRibbon
    {
        private void TarjimonRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            btnLatinToCyrillic.Image = ResourceLoader.LatinToCyrillic;
            btnCyrillicToLatin.Image = ResourceLoader.CyrillicToLatin;
            btnADX.Image = ResourceLoader.Settings;
        }

        private Worksheet GetActiveWorksheet()
        {
            return Globals.ThisAddIn.Application.ActiveSheet as Worksheet;
        }

        private Range GetSelectedRange()
        {
            return Globals.ThisAddIn.Application.Selection as Range;
        }

        private Range GetTranslationRange()
        {
            Range selectedRange = GetSelectedRange();

            if (selectedRange == null)
                return null;

            // Excel always has an active cell. A single-cell selection is treated
            // as "no explicit range" and translates the worksheet's used range.
            if (selectedRange.Cells.CountLarge == 1)
            {
                Worksheet worksheet = GetActiveWorksheet();

                if (worksheet == null)
                    return null;

                Range usedRange = worksheet.UsedRange;

                if (usedRange == null || usedRange.Cells.CountLarge == 1)
                    return selectedRange;

                return usedRange;
            }

            // Explicit multi-cell selection: translate only what the user selected.
            return selectedRange;
        }

        private string NormalizeExcelCyrillicInput(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Excel-specific safeguard for the Uzbek spelling Оъ/оъ.
            // This must become O'/o' when Cyrillic is converted to Latin.
            return text
                .Replace("Оъ", "О'")
                .Replace("оъ", "о'")
                .Replace("ОЪ", "О'")
                .Replace("оЪ", "о'");
        }

        private void ConvertSelectedCells(bool latinToCyrillic)
        {
            Range translationRange = GetTranslationRange();

            if (translationRange == null)
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

            foreach (Range cell in translationRange.Cells)
            {
                try
                {
                    if (cell.HasFormula)
                        continue;

                    object value = cell.Value2;

                    if (value == null)
                        continue;

                    string text = Convert.ToString(value);

                    if (text == null || string.IsNullOrWhiteSpace(text))
                        continue;

                    if (!latinToCyrillic)
                        text = NormalizeExcelCyrillicInput(text);

                    string result = latinToCyrillic
                        ? Transliterator.LatinToCyrillic(text)
                        : Transliterator.CyrillicToLatin(text);

                    if (!string.Equals(result, Convert.ToString(value), StringComparison.Ordinal))
                        cell.Value2 = result;
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
            using (SettingsForm form = new SettingsForm())
            {
                form.ShowDialog();
            }
        }
    }
}

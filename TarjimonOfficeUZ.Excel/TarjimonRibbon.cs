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

        // Excel translation has exactly two modes:
        // 1) one active cell selected -> translate the worksheet's used data area;
        // 2) two or more cells selected -> translate only that selection.
        private Range GetTranslationRange()
        {
            Worksheet worksheet = GetActiveWorksheet();
            Range selectedRange = GetSelectedRange();

            if (worksheet == null || selectedRange == null)
                return null;

            if (selectedRange.Cells.CountLarge <= 1)
                return worksheet.UsedRange;

            return selectedRange;
        }

        private string NormalizeExcelCyrillicInput(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Excel-specific safeguard for the Uzbek spelling Оъ/оъ.
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

            dynamic undoRecord = null;
            bool undoStarted = false;
            int changedCells = 0;

            try
            {
                // Excel groups all cell changes made between StartCustomRecord and
                // EndCustomRecord into one Ctrl+Z operation.
                undoRecord = Globals.ThisAddIn.Application.UndoRecord;
                string undoTitle = latinToCyrillic
                    ? "Lotin → Kirill tarjimasi"
                    : "Kirill → Lotin tarjimasi";
                undoRecord.StartCustomRecord(undoTitle);
                undoStarted = true;

                foreach (Range cell in translationRange.Cells)
                {
                    try
                    {
                        // Rules: formulas and empty cells are never translated.
                        if (cell.HasFormula)
                            continue;

                        object value = cell.Value2;

                        if (value == null)
                            continue;

                        string text = Convert.ToString(value);

                        if (string.IsNullOrWhiteSpace(text))
                            continue;

                        string originalText = text;

                        if (!latinToCyrillic)
                            text = NormalizeExcelCyrillicInput(text);

                        string result = latinToCyrillic
                            ? Transliterator.LatinToCyrillic(text)
                            : Transliterator.CyrillicToLatin(text);

                        if (!string.Equals(result, originalText, StringComparison.Ordinal))
                        {
                            cell.Value2 = result;
                            changedCells++;
                        }
                    }
                    catch
                    {
                        // One problematic cell must not stop the remaining cells.
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Excel tarjimasi bajarildi, lekin Undo yozuvini yaratishda muammo yuz berdi.\n\n" +
                    ex.Message,
                    "ADX Office",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                if (undoStarted && undoRecord != null)
                {
                    try
                    {
                        undoRecord.EndCustomRecord();
                    }
                    catch
                    {
                        // Do not replace a successful translation with an Undo cleanup error.
                    }
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

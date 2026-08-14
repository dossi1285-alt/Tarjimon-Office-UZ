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

        // Frozen Excel rules:
        // 1) one active cell -> translate the worksheet's used data area;
        // 2) two or more selected cells -> translate only that selection.
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

            // One translation click = one undo operation. The snapshot is replaced
            // before every new translation so older translation states are not mixed.
            ExcelTranslationUndoManager.Clear();

            // The translation itself is frozen and remains unchanged.
            // Before changing cells, capture only the cells that will actually change.
            int changedCount = TranslateRangeCells(translationRange, latinToCyrillic, true);

            if (changedCount == 0)
            {
                ExcelTranslationUndoManager.Clear();
                return;
            }

            string undoTitle = latinToCyrillic
                ? "Lotin → Kirill tarjimasini bekor qilish"
                : "Kirill → Lotin tarjimasini bekor qilish";

            // Excel's native VBA OnUndo mechanism is the bridge point. The VBA bridge
            // calls the COM automation method exposed by ThisAddIn.
            try
            {
                dynamic excelApplication = Globals.ThisAddIn.Application;
                excelApplication.OnUndo(undoTitle, "TarjimonOfficeUZ.UndoBridge.xlam!UndoLastTranslation");
            }
            catch
            {
                // The bridge may not be installed yet. The snapshot is still kept so
                // the bridge can use it as soon as it is installed.
            }
        }

        private int TranslateRangeCells(Range translationRange, bool latinToCyrillic, bool captureUndo)
        {
            int changedCount = 0;

            foreach (Range cell in translationRange.Cells)
            {
                try
                {
                    // Fixed Excel rules: formulas and empty cells are never translated.
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
                        if (captureUndo)
                        {
                            ExcelTranslationUndoManager.CaptureCell(
                                cell,
                                value);
                        }

                        cell.Value2 = result;
                        changedCount++;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return changedCount;
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

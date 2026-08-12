using Microsoft.Office.Interop.Word;

namespace TarjimonOfficeUZ.Word.Helpers
{
    internal static class WordRangeHelper
    {
        internal static string ReadText(Range range)
        {
            if (range == null)
                return string.Empty;

            string text = range.Text;

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.EndsWith("\r\a"))
                return text.Substring(0, text.Length - 2);

            if (text.EndsWith("\r"))
                return text.Substring(0, text.Length - 1);

            return text;
        }

        internal static bool IsTableCell(Range range)
        {
            if (range == null)
                return false;

            return range.Information[WdInformation.wdWithInTable];
        }

        internal static void ReplaceText(Range range, string newText)
        {
            if (range == null)
                return;

            if (newText == null)
                newText = string.Empty;

            Range textRange = null;

            try
            {
                textRange = range.Duplicate;

                if (IsTableCell(textRange))
                {
                    // Katak oxiridagi markerlarni saqlab qolamiz
                    textRange.MoveEnd(
                        WdUnits.wdCharacter,
                        -1);

                    if (textRange.Text.EndsWith("\r"))
                    {
                        textRange.MoveEnd(
                            WdUnits.wdCharacter,
                            -1);
                    }
                }
                else
                {
                    if (textRange.Text.EndsWith("\r"))
                    {
                        textRange.MoveEnd(
                            WdUnits.wdCharacter,
                            -1);
                    }
                }

                textRange.Text = newText;
            }
            finally
            {
                if (textRange != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(textRange);
                    textRange = null;
                }
            }
        }
        
    }
}
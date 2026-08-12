using Microsoft.Office.Interop.Word;

namespace TarjimonOfficeUZ.Word.Helpers
{
    internal sealed class FormatSegment
    {
        public int Start { get; set; }

        public int Length { get; set; }
        public int OriginalTextStart { get; set; }

        public string Text { get; set; } = string.Empty;

        public Font Font { get; set; }
        public void Dispose()
        {
            if (Font != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Font);
                Font = null;
            }
        }
    }
}
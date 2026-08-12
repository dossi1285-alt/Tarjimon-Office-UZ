using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Office.Tools.Word;
using TarjimonOfficeUZ.Core.Translation;
using Word = Microsoft.Office.Interop.Word;
namespace TarjimonOfficeUZ.Word
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, EventArgs e)
        {

        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {

        }


        public void ConvertSelectionLatinToCyrillic()
        {
            if (TryUndoLastCyrillicToLatin())
                return;

            var selection = this.Application.Selection;

            if (selection == null)
                return;

            string originalText = null;
            string translatedText = null;

            bool wholeDocument = false;
            int translatedStart = -1;
            int translatedEnd = -1;

            var doc = this.Application.ActiveDocument;

            if (doc == null)
                return;

            string documentKey = doc.FullName;

            this.Application.UndoRecord.StartCustomRecord(
                "Latin → Cyrillic");

            try
            {
                if (selection.Start == selection.End)
                {
                    wholeDocument = true;

                    originalText = doc.Content.Text;

                    ConvertDocumentLatinToCyrillic();

                    translatedText = doc.Content.Text;
                }
                else
                {
                    var range = selection.Range.Duplicate;

                    originalText = range.Text;

                    ConvertSelectionRange(
                        range,
                        Transliterator.LatinToCyrillic);

                    translatedText = selection.Range.Text;

                    translatedStart = selection.Start;
                    translatedEnd = selection.End;

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(
                        range);
                }

                if (!string.Equals(
                    originalText,
                    translatedText,
                    StringComparison.Ordinal))
                {
                    ReverseTranslationCache.OriginalText =
                        originalText;

                    ReverseTranslationCache.TranslatedText =
                        translatedText;

                    ReverseTranslationCache.Direction =
                        TranslationDirection.LatinToCyrillic;

                    ReverseTranslationCache.DocumentKey =
                        documentKey;

                    ReverseTranslationCache.RangeStart =
                        translatedStart;

                    ReverseTranslationCache.RangeEnd =
                        translatedEnd;

                    ReverseTranslationCache.IsWholeDocument =
                        wholeDocument;
                }
            }
            finally
            {
                this.Application.UndoRecord.EndCustomRecord();
            }
        }
        private bool TryUndoLastLatinToCyrillic()
        {
            if (ReverseTranslationCache.Direction !=
                TranslationDirection.LatinToCyrillic)
            {
                return false;
            }

            var doc = this.Application.ActiveDocument;

            if (doc == null)
                return false;

            // Faqat o'sha hujjatda ishlasin
            if (!string.Equals(
                doc.FullName,
                ReverseTranslationCache.DocumentKey,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var selection = this.Application.Selection;

            if (selection == null)
                return false;

            // Butun hujjat tarjima qilingan holat
            if (ReverseTranslationCache.IsWholeDocument)
            {
                // Faqat butun hujjat tanlangan bo'lsa,
                // butun hujjatning oldingi tarjimasini Undo qilamiz.
                if (selection.Start != doc.Content.Start ||
                    selection.End != doc.Content.End)
                {
                    return false;
                }

                string currentDocumentText =
                    doc.Content.Text;

                if (!string.Equals(
                    currentDocumentText,
                    ReverseTranslationCache.TranslatedText,
                    StringComparison.Ordinal))
                {
                    return false;
                }

                this.Application.CommandBars.ExecuteMso("Undo");

                ReverseTranslationCache.Clear();

                return true;
            }

            // Faqat aynan o'sha Range bo'lsa Undo
            if (selection.Start !=
                ReverseTranslationCache.RangeStart)
            {
                return false;
            }

            if (selection.End !=
                ReverseTranslationCache.RangeEnd)
            {
                return false;
            }

            string currentSelectionText =
                selection.Range.Text;

            if (!string.Equals(
                currentSelectionText,
                ReverseTranslationCache.TranslatedText,
                StringComparison.Ordinal))
            {
                return false;
            }

            this.Application.CommandBars.ExecuteMso("Undo");

            ReverseTranslationCache.Clear();

            return true;
        }
        private bool TryUndoLastCyrillicToLatin()
        {
            if (ReverseTranslationCache.Direction !=
                TranslationDirection.CyrillicToLatin)
            {
                return false;
            }

            var doc = this.Application.ActiveDocument;

            if (doc == null)
                return false;

            // Faqat o'sha hujjatda ishlasin
            if (!string.Equals(
                doc.FullName,
                ReverseTranslationCache.DocumentKey,
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var selection = this.Application.Selection;

            if (selection == null)
                return false;

            // Butun hujjat tarjima qilingan holat
            if (ReverseTranslationCache.IsWholeDocument)
            {
                // Agar foydalanuvchi butun hujjatni emas,
                // faqat bir qismini belgilagan bo'lsa,
                // oldingi butun-hujjat tarjimasini Undo qilmaymiz.
                if (selection.Start != doc.Content.Start ||
                    selection.End != doc.Content.End)
                {
                    return false;
                }

                string currentDocumentText =
                    doc.Content.Text;

                if (!string.Equals(
                    currentDocumentText,
                    ReverseTranslationCache.TranslatedText,
                    StringComparison.Ordinal))
                {
                    return false;
                }

                this.Application.CommandBars.ExecuteMso("Undo");

                ReverseTranslationCache.Clear();

                return true;
            }

            // Faqat aynan o'sha Range bo'lsa Undo
            if (selection.Start !=
                ReverseTranslationCache.RangeStart)
            {
                return false;
            }

            if (selection.End !=
                ReverseTranslationCache.RangeEnd)
            {
                return false;
            }

            string currentSelectionText =
                selection.Range.Text;

            if (!string.Equals(
                currentSelectionText,
                ReverseTranslationCache.TranslatedText,
                StringComparison.Ordinal))
            {
                return false;
            }

            this.Application.CommandBars.ExecuteMso("Undo");

            ReverseTranslationCache.Clear();

            return true;
        }

        public void ConvertSelectionCyrillicToLatin()
        {
            if (TryUndoLastLatinToCyrillic())
                return;

            var selection = this.Application.Selection;

            if (selection == null)
                return;

            string originalText = null;
            string translatedText = null;

            bool wholeDocument = false;
            int translatedStart = -1;
            int translatedEnd = -1;

            var doc = this.Application.ActiveDocument;

            if (doc == null)
                return;

            string documentKey = doc.FullName;

            this.Application.UndoRecord.StartCustomRecord(
                "Cyrillic → Latin");

            try
            {
                if (selection.Start == selection.End)
                {
                    wholeDocument = true;

                    originalText = doc.Content.Text;

                    ConvertDocumentCyrillicToLatin();

                    translatedText = doc.Content.Text;
                }
                else
                {
                    var range = selection.Range.Duplicate;

                    originalText = range.Text;

                    ConvertSelectionRange(
                        range,
                        Transliterator.CyrillicToLatin);

                    translatedText = selection.Range.Text;

                    translatedStart = selection.Start;
                    translatedEnd = selection.End;

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(
                        range);
                }

                if (!string.Equals(
                    originalText,
                    translatedText,
                    StringComparison.Ordinal))
                {
                    ReverseTranslationCache.OriginalText =
                        originalText;

                    ReverseTranslationCache.TranslatedText =
                        translatedText;

                    ReverseTranslationCache.Direction =
                        TranslationDirection.CyrillicToLatin;

                    ReverseTranslationCache.DocumentKey =
                        documentKey;

                    ReverseTranslationCache.RangeStart =
                        translatedStart;

                    ReverseTranslationCache.RangeEnd =
                        translatedEnd;

                    ReverseTranslationCache.IsWholeDocument =
                        wholeDocument;
                }
            }
            finally
            {
                this.Application.UndoRecord.EndCustomRecord();
            }
        }

        public void ConvertDocumentLatinToCyrillic()
        {
            ConvertParagraphs(Transliterator.LatinToCyrillic);
            ConvertTableCells(Transliterator.LatinToCyrillic);
        }

        public void ConvertDocumentCyrillicToLatin()
        {
            ConvertParagraphs(Transliterator.CyrillicToLatin);
            ConvertTableCells(Transliterator.CyrillicToLatin);
        }
        private void ConvertParagraphs(Func<string, string> converter)
        {
            var doc = this.Application.ActiveDocument;

            if (doc == null)
                return;

            foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in doc.Paragraphs)
            {
                if (paragraph.Range.Information[
                    Microsoft.Office.Interop.Word.WdInformation.wdWithInTable])
                {
                    continue;
                }
                
                
                var range = paragraph.Range;
                var workRange = range.Duplicate;

                string originalText = Word.Helpers.WordRangeHelper.ReadText(workRange);

                if (string.IsNullOrWhiteSpace(originalText))
                    continue;

                var segments =
    Word.Helpers.FormatCollector.Collect(workRange);

                TranslationResult translation;

                if (converter == Transliterator.LatinToCyrillic)
                {
                    translation = Transliterator.LatinToCyrillicWithMapping(originalText);
                }
                else if (converter == Transliterator.CyrillicToLatin)
                {
                    translation = Transliterator.CyrillicToLatinWithMapping(originalText);
                }
                else
                {
                    throw new InvalidOperationException("Unknown converter.");
                }

                string convertedText = translation.Text;

                if (string.Equals(originalText, convertedText, StringComparison.Ordinal))
                {
                    foreach (var segment in segments)
                        segment.Dispose();

                    continue;
                }

                Word.Helpers.WordRangeHelper.ReplaceText(
    workRange,
    convertedText);

                segments = Word.Helpers.SegmentMapper.Map(
                    segments,
                    translation);

                Word.Helpers.FormatApplier.Apply(
                    workRange,
                    segments,
                    translation);
            }
        }
        private void ConvertTableCells(Func<string, string> converter)
        {
            var doc = this.Application.ActiveDocument;

            if (doc == null)
                return;

            foreach (Microsoft.Office.Interop.Word.Table table in doc.Tables)
            {
                foreach (Microsoft.Office.Interop.Word.Row row in table.Rows)
                {
                    foreach (Microsoft.Office.Interop.Word.Cell cell in row.Cells)
                    {
                        var workRange = cell.Range.Duplicate;
                        
                        string originalText =
                            Word.Helpers.WordRangeHelper.ReadText(workRange);

                        if (string.IsNullOrWhiteSpace(originalText))
                            continue;

                        var segments =
    Word.Helpers.FormatCollector.Collect(workRange);

                        TranslationResult translation;

                        if (converter == Transliterator.LatinToCyrillic)
                        {
                            translation = Transliterator.LatinToCyrillicWithMapping(originalText);
                        }
                        else if (converter == Transliterator.CyrillicToLatin)
                        {
                            translation = Transliterator.CyrillicToLatinWithMapping(originalText);
                        }
                        else
                        {
                            throw new InvalidOperationException("Unknown converter.");
                        }

                        string convertedText = translation.Text;

                        if (string.Equals(originalText,
                                          convertedText,
                                          StringComparison.Ordinal))
                        {
                            foreach (var segment in segments)
                                segment.Dispose();

                            continue;
                        }

                        Word.Helpers.WordRangeHelper.ReplaceText(
    workRange,
    convertedText);

                        segments = Word.Helpers.SegmentMapper.Map(
                            segments,
                            translation);

                        Word.Helpers.FormatApplier.Apply(
                            workRange,
                            segments,
                            translation);
                    }
                }
            }
        }

        private void ConvertSelectionRange(
    Microsoft.Office.Interop.Word.Range selectionRange,
    Func<string, string> converter)
        {
            if (selectionRange == null)
                return;

            if (converter == null)
                return;

            var ranges = new System.Collections.Generic.List<Microsoft.Office.Interop.Word.Range>();

            foreach (Microsoft.Office.Interop.Word.Paragraph paragraph in selectionRange.Paragraphs)
            {
                Microsoft.Office.Interop.Word.Range workRange = paragraph.Range.Duplicate;

                if (workRange.End <= selectionRange.Start)
                    continue;

                if (workRange.Start >= selectionRange.End)
                    continue;

                if (workRange.Start < selectionRange.Start)
                    workRange.Start = selectionRange.Start;

                if (workRange.End > selectionRange.End)
                    workRange.End = selectionRange.End;

                ranges.Add(workRange);
            }

            try
            {
                foreach (Microsoft.Office.Interop.Word.Cell cell in selectionRange.Cells)
                {
                    Microsoft.Office.Interop.Word.Range workRange = cell.Range.Duplicate;

                    if (workRange.End <= selectionRange.Start)
                        continue;

                    if (workRange.Start >= selectionRange.End)
                        continue;

                    if (workRange.Start < selectionRange.Start)
                        workRange.Start = selectionRange.Start;

                    if (workRange.End > selectionRange.End)
                        workRange.End = selectionRange.End;

                    ranges.Add(workRange);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                // Selection ichida jadval yo'q.
            }
            ranges = ranges
    .GroupBy(r => r.Start.ToString() + "_" + r.End.ToString())
    .Select(g => g.First())
    .OrderByDescending(r => r.Start)
    .ToList();
            foreach (Microsoft.Office.Interop.Word.Range workRange in ranges)
            {
                string originalText =
                    Word.Helpers.WordRangeHelper.ReadText(workRange);

                if (string.IsNullOrWhiteSpace(originalText))
                    continue;

                var segments =
     Word.Helpers.FormatCollector.Collect(workRange);

                TranslationResult translation;

                if (converter == Transliterator.LatinToCyrillic)
                {
                    translation = Transliterator.LatinToCyrillicWithMapping(originalText);
                }
                else if (converter == Transliterator.CyrillicToLatin)
                {
                    translation = Transliterator.CyrillicToLatinWithMapping(originalText);
                }
                else
                {
                    throw new InvalidOperationException("Unknown converter.");
                }

                string convertedText = translation.Text;

                if (string.Equals(originalText,
                                  convertedText,
                                  StringComparison.Ordinal))
                {
                    foreach (var segment in segments)
                        segment.Dispose();

                    continue;
                }

                Word.Helpers.WordRangeHelper.ReplaceText(
    workRange,
    convertedText);

                segments = Word.Helpers.SegmentMapper.Map(
                    segments,
                    translation);

                Word.Helpers.FormatApplier.Apply(
                    workRange,
                    segments,
                    translation);
            }
        }
        #region Код, автоматически созданный VSTO

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new EventHandler(ThisAddIn_Startup);
            this.Shutdown += new EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
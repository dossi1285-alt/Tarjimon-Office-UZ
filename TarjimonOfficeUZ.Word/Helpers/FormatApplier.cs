using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace TarjimonOfficeUZ.Word.Helpers
{
    internal static class FormatApplier
    {
        internal static void Apply(
    Range range,
    List<FormatSegment> segments,
    TarjimonOfficeUZ.Core.Translation.TranslationResult translation)
    {
            if (range == null)
                return;

            if (segments == null)
                return;

            if (segments.Count == 0)
                return;
            if (translation == null)
                return;

            Range workRange = null;
            Range segmentRange = null;

            try
            {
                workRange = range.Duplicate;

                foreach (FormatSegment segment in segments)
                {
                    System.Diagnostics.Debug.WriteLine(
    $"Range: {segment.Start} - {segment.Start + segment.Length}");
                    segmentRange = workRange.Duplicate;

                    segmentRange.SetRange(
    workRange.Start + segment.Start,
    workRange.Start + segment.Start + segment.Length);
                    System.Diagnostics.Debug.WriteLine(
    $"Word: {segmentRange.Start} - {segmentRange.End}");

                    ApplyFont(
                        segmentRange,
                        segment.Font);

                    Marshal.ReleaseComObject(segmentRange);
                    segmentRange = null;
                }
            }
            finally
            {
                if (segmentRange != null)
                {
                    Marshal.ReleaseComObject(segmentRange);
                    segmentRange = null;
                }

                if (workRange != null)
                {
                    Marshal.ReleaseComObject(workRange);
                    workRange = null;
                }

                if (segments != null)
                {
                    foreach (FormatSegment segment in segments)
                    {
                        if (segment != null)
                            segment.Dispose();
                    }

                    segments.Clear();
                }
            }
        }


        
        private static void ApplyFont(
            Range range,
            Font font)
        {
            if (range == null)
                return;

            if (font == null)
                return;

            range.Font.Name = font.Name;
            range.Font.Size = font.Size;
            range.Font.Bold = font.Bold;
            range.Font.Italic = font.Italic;
            range.Font.Underline = font.Underline;
            range.Font.Color = font.Color;
            range.Font.Subscript = font.Subscript;
            range.Font.Superscript = font.Superscript;
            range.Font.StrikeThrough = font.StrikeThrough;
            range.Font.DoubleStrikeThrough = font.DoubleStrikeThrough;
            range.Font.SmallCaps = font.SmallCaps;
            range.Font.AllCaps = font.AllCaps;
            range.Font.Hidden = font.Hidden;
            range.Font.Kerning = font.Kerning;
            range.Font.Spacing = font.Spacing;
            range.Font.Scaling = font.Scaling;
            range.Font.Position = font.Position;
            range.Font.Emboss = font.Emboss;
            range.Font.Engrave = font.Engrave;
            range.Font.Outline = font.Outline;
            range.Font.Shadow = font.Shadow;
            range.Font.NameAscii = font.NameAscii;
        }
    }
}
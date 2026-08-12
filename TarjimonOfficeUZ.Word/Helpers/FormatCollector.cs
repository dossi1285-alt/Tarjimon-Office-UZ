using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TarjimonOfficeUZ.Word.Helpers
{
    internal static class FormatCollector
    {
        internal static List<FormatSegment> Collect(Range range)
        {
            List<FormatSegment> segments = new List<FormatSegment>();

            if (range == null)
                return segments;

            Range workRange = range.Duplicate;

            int position = workRange.Start;

            while (position < workRange.End)
            {
                int segmentEnd = FindSegmentEnd(
                    workRange,
                    position);

                Range segmentRange = workRange.Duplicate;

                segmentRange.SetRange(
                    position,
                    segmentEnd);

                AddSegment(
                    segments,
                    segmentRange,
                    workRange);

                Marshal.ReleaseComObject(segmentRange);

                position = segmentEnd;
            }

            Marshal.ReleaseComObject(workRange);

            return segments;
        }

        private static FormatSegment CreateSegment(
    Range range,
    Range workRange)
        {
            Font sourceFont = null;

            try
            {
                FormatSegment segment = new FormatSegment();

                segment.Start = range.Start;
                segment.Length = range.End - range.Start;

                if (segment.Length <= 0)
                    return null;

                segment.Text = range.Text;

                segment.OriginalTextStart =
                    range.Start - workRange.Start;

                sourceFont = range.Font;
                segment.Font = sourceFont.Duplicate;

                return segment;
            }
            finally
            {
                if (sourceFont != null)
                {
                    Marshal.ReleaseComObject(sourceFont);
                    sourceFont = null;
                }
            }
        }
        private static void AddSegment(
    List<FormatSegment> segments,
    Range range,
    Range workRange)
        {
            if (range == null)
                return;

            if (range.Start >= range.End)
                return;

            FormatSegment segment =
                CreateSegment(range, workRange);

            if (segment != null)
                segments.Add(segment);
        }

        private static int FindSegmentEnd(
     Range workRange,
     int startPosition)
        {
            Font baseFont = null;
            Range charRange = null;

            try
            {
                charRange = workRange.Duplicate;

                charRange.SetRange(
                    startPosition,
                    startPosition + 1);

                baseFont = charRange.Font.Duplicate;

                int position = startPosition + 1;

                while (position < workRange.End)
                {
                    charRange.SetRange(
                        position,
                        position + 1);

                    if (!IsSameFont(
                        baseFont,
                        charRange.Font))
                    {
                        return position;
                    }

                    position++;
                }

                return workRange.End;
            }
            finally
            {
                if (baseFont != null)
                    Marshal.ReleaseComObject(baseFont);

                if (charRange != null)
                    Marshal.ReleaseComObject(charRange);
            }
        }
        private static bool IsSameFont(
    Font firstFont,
    Font secondFont)
        {
            return firstFont.Name == secondFont.Name
    && firstFont.Size == secondFont.Size
    && firstFont.Bold == secondFont.Bold
    && firstFont.Italic == secondFont.Italic
    && firstFont.Underline == secondFont.Underline
    && firstFont.Color == secondFont.Color
    && firstFont.Subscript == secondFont.Subscript
    && firstFont.Superscript == secondFont.Superscript
    && firstFont.StrikeThrough == secondFont.StrikeThrough
    && firstFont.DoubleStrikeThrough == secondFont.DoubleStrikeThrough
    && firstFont.SmallCaps == secondFont.SmallCaps
    && firstFont.AllCaps == secondFont.AllCaps
    && firstFont.Hidden == secondFont.Hidden
&& firstFont.Kerning == secondFont.Kerning
&& firstFont.Spacing == secondFont.Spacing
&& firstFont.Scaling == secondFont.Scaling
&& firstFont.Position == secondFont.Position
&& firstFont.Emboss == secondFont.Emboss
&& firstFont.Engrave == secondFont.Engrave
&& firstFont.Outline == secondFont.Outline
&& firstFont.Shadow == secondFont.Shadow
&& firstFont.BoldBi == secondFont.BoldBi
&& firstFont.ItalicBi == secondFont.ItalicBi
&& firstFont.NameBi == secondFont.NameBi
&& firstFont.SizeBi == secondFont.SizeBi


&& firstFont.NameFarEast == secondFont.NameFarEast


&& firstFont.NameAscii == secondFont.NameAscii

&& firstFont.NameOther == secondFont.NameOther


&& firstFont.Animation == secondFont.Animation
&& firstFont.DisableCharacterSpaceGrid == secondFont.DisableCharacterSpaceGrid;

        }
        private static void DisposeSegments(List<FormatSegment> segments)
        {
            if (segments == null)
                return;

            foreach (FormatSegment segment in segments)
            {
                if (segment != null)
                    segment.Dispose();
            }

            segments.Clear();
        }
    }
}
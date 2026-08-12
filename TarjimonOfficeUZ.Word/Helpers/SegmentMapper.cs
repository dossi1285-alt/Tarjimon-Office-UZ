using System.Collections.Generic;
using System.Linq;
using TarjimonOfficeUZ.Core.Translation;

namespace TarjimonOfficeUZ.Word.Helpers
{
    internal static class SegmentMapper
    {
        internal static List<FormatSegment> Map(
            List<FormatSegment> segments,
            TranslationResult translation)
        {
            if (segments == null)
                return segments;

            if (translation == null)
                return segments;

            /*
             * Har bir yangi belgi faqat BITTA format segmentiga
             * tegishli bo'lishi kerak.
             *
             * Masalan:
             *
             *   "sh" -> "ш"
             *
             * CharacterMap ichida "ш" uchun ikkita source index
             * bo'lishi mumkin: s va h.
             *
             * Oldingi kodda shu "ш" ikkala segmentga ham tushib,
             * FormatApplier tomonidan ikki marta formatlanardi.
             *
             * Endi yangi belgi uning BIRINCHI source index'i
             * joylashgan segmentga biriktiriladi.
             */

            foreach (FormatSegment segment in segments)
            {
                int sourceStart = segment.OriginalTextStart;
                int sourceEnd =
                    sourceStart + segment.Text.Length - 1;

                List<int> mappedIndexes = new List<int>();

                foreach (CharacterMap item in translation.Mapping)
                {
                    if (item == null)
                        continue;

                    if (item.SourceIndexes == null ||
                        item.SourceIndexes.Count == 0)
                        continue;

                    /*
                     * Bitta yangi belgini hosil qilgan bir nechta
                     * eski belgilar ichidan birinchisini "egasi"
                     * deb olamiz.
                     *
                     * Masalan:
                     *
                     *   g' -> ғ
                     *
                     * "ғ" uchun g va ' berilgan bo'lsa,
                     * birinchi source index — g.
                     */
                    int ownerSourceIndex =
                        item.SourceIndexes.Min();

                    if (ownerSourceIndex < sourceStart ||
                        ownerSourceIndex > sourceEnd)
                    {
                        continue;
                    }

                    mappedIndexes.Add(item.NewIndex);
                }

                if (mappedIndexes.Count == 0)
                    continue;

                mappedIndexes.Sort();

                int newStart = mappedIndexes.First();
                int newEnd = mappedIndexes.Last();

                segment.Start = newStart;
                segment.Length =
                    newEnd - newStart + 1;
            }

            return segments;
        }
    }
}
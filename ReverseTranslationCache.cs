using System;

namespace TarjimonOfficeUZ.Core.Translation
{
    public static class ReverseTranslationCache
    {
        public static string OriginalText { get; set; }

        public static string TranslatedText { get; set; }

        public static TranslationDirection Direction { get; set; }

        // Oldingi tarjima qilingan Word hujjati
        public static string DocumentKey { get; set; }

        // Tarjima qilingan Range koordinatalari
        public static int RangeStart { get; set; }

        public static int RangeEnd { get; set; }

        // Butun hujjat tarjima qilingan bo'lsa true
        public static bool IsWholeDocument { get; set; }

        public static void Clear()
        {
            OriginalText = null;
            TranslatedText = null;
            Direction = TranslationDirection.None;

            DocumentKey = null;

            RangeStart = -1;
            RangeEnd = -1;

            IsWholeDocument = false;
        }
    }
}
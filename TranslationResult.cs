using System.Collections.Generic;

namespace TarjimonOfficeUZ.Core.Translation
{
    public sealed class CharacterMap
    {
        // Yangi matndagi belgi indeksi
        public int NewIndex { get; set; }

        // Shu belgini hosil qilgan eski indekslar
        public List<int> SourceIndexes { get; } = new List<int>();
    }

    public sealed class TranslationResult
    {
        // Tarjima qilingan matn
        public string Text { get; set; } = string.Empty;

        // Eski -> yangi bog'lanish
        public List<CharacterMap> Mapping { get; } = new List<CharacterMap>();
    }
}
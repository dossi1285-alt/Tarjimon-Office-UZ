using System;
using System.Collections.Generic;

namespace TarjimonOfficeUZ.Core.Translation
{
    /// <summary>
    /// Tarjimon Office UZ uchun lotin-kirill transliteratsiya qoidalari.
    /// Kontekstga bog'liq E/e va Е/е qoidalari Transliterator.cs ichida
    /// alohida boshqariladi.
    /// </summary>
    public static class CurrentAlphabet
    {
        /// <summary>
        /// Lotin -> Kirill asosiy qoidalar.
        /// </summary>
        public static readonly IReadOnlyList<AlphabetRule> LatinToCyrillic =
            new List<AlphabetRule>
            {
                // ============================================
                // MAXSUS O' / G'
                // ============================================

                new AlphabetRule("O'", "Ў"),
                new AlphabetRule("o'", "ў"),

                new AlphabetRule("G'", "Ғ"),
                new AlphabetRule("g'", "ғ"),

                // ============================================
                // APOSTROFNING BOSHQA UNICODE KO'RINISHLARI
                // NormalizeText ularni ' ga aylantiradi,
                // lekin xavfsizlik uchun qoldirilmoqda.
                // ============================================

                new AlphabetRule("Oʻ", "Ў"),
                new AlphabetRule("oʻ", "ў"),
                new AlphabetRule("O’", "Ў"),
                new AlphabetRule("o’", "ў"),
                new AlphabetRule("Oʼ", "Ў"),
                new AlphabetRule("oʼ", "ў"),

                new AlphabetRule("Gʻ", "Ғ"),
                new AlphabetRule("gʻ", "ғ"),
                new AlphabetRule("G’", "Ғ"),
                new AlphabetRule("g’", "ғ"),
                new AlphabetRule("Gʼ", "Ғ"),
                new AlphabetRule("gʼ", "ғ"),

                new AlphabetRule("O‘", "Ў"),
                new AlphabetRule("o‘", "ў"),
                new AlphabetRule("G‘", "Ғ"),
                new AlphabetRule("g‘", "ғ"),

                new AlphabetRule("O´", "Ў"),
                new AlphabetRule("o´", "ў"),
                new AlphabetRule("G´", "Ғ"),
                new AlphabetRule("g´", "ғ"),

                new AlphabetRule("O`", "Ў"),
                new AlphabetRule("o`", "ў"),
                new AlphabetRule("G`", "Ғ"),
                new AlphabetRule("g`", "ғ"),

                // ============================================
                // YO' = ЙЎ
                // Muhim: YO' oddiy YO dan oldin tekshiriladi.
                // ============================================

                new AlphabetRule("YO'", "ЙЎ"),
                new AlphabetRule("Yo'", "Йў"),
                new AlphabetRule("yo'", "йў"),

                // ============================================
                // YE / YA / YO / YU
                // ============================================

                new AlphabetRule("YE", "Е"),
                new AlphabetRule("Ye", "Е"),
                new AlphabetRule("ye", "е"),

                new AlphabetRule("YA", "Я"),
                new AlphabetRule("Ya", "Я"),
                new AlphabetRule("ya", "я"),

                new AlphabetRule("YO", "Ё"),
                new AlphabetRule("Yo", "Ё"),
                new AlphabetRule("yo", "ё"),

                new AlphabetRule("YU", "Ю"),
                new AlphabetRule("Yu", "Ю"),
                new AlphabetRule("yu", "ю"),

                // ============================================
                // SH / CH / NG
                // ============================================

                new AlphabetRule("SH", "Ш"),
                new AlphabetRule("Sh", "Ш"),
                new AlphabetRule("sh", "ш"),

                new AlphabetRule("CH", "Ч"),
                new AlphabetRule("Ch", "Ч"),
                new AlphabetRule("ch", "ч"),

                new AlphabetRule("NG'", "НҒ"),
                new AlphabetRule("Ng'", "Нғ"),
                new AlphabetRule("ng'", "нғ"),

                new AlphabetRule("NG", "НГ"),
                new AlphabetRule("Ng", "Нг"),
                new AlphabetRule("ng", "нг"),

                // ============================================
                // TS
                // ============================================

                new AlphabetRule("TS", "Ц"),
                new AlphabetRule("Ts", "Ц"),
                new AlphabetRule("ts", "ц"),

                // ============================================
                // ASOSIY HARFLAR
                // E/e BU YERDA YO'Q.
                // Uni Transliterator kontekst bo'yicha hal qiladi.
                // ============================================

                new AlphabetRule("A", "А"),
                new AlphabetRule("a", "а"),

                new AlphabetRule("B", "Б"),
                new AlphabetRule("b", "б"),

                new AlphabetRule("D", "Д"),
                new AlphabetRule("d", "д"),

                new AlphabetRule("F", "Ф"),
                new AlphabetRule("f", "ф"),

                new AlphabetRule("G", "Г"),
                new AlphabetRule("g", "г"),

                new AlphabetRule("H", "Ҳ"),
                new AlphabetRule("h", "ҳ"),

                new AlphabetRule("I", "И"),
                new AlphabetRule("i", "и"),

                new AlphabetRule("J", "Ж"),
                new AlphabetRule("j", "ж"),

                new AlphabetRule("K", "К"),
                new AlphabetRule("k", "к"),

                new AlphabetRule("L", "Л"),
                new AlphabetRule("l", "л"),

                new AlphabetRule("M", "М"),
                new AlphabetRule("m", "м"),

                new AlphabetRule("N", "Н"),
                new AlphabetRule("n", "н"),

                new AlphabetRule("O", "О"),
                new AlphabetRule("o", "о"),

                new AlphabetRule("P", "П"),
                new AlphabetRule("p", "п"),

                new AlphabetRule("Q", "Қ"),
                new AlphabetRule("q", "қ"),

                new AlphabetRule("R", "Р"),
                new AlphabetRule("r", "р"),

                new AlphabetRule("S", "С"),
                new AlphabetRule("s", "с"),

                new AlphabetRule("T", "Т"),
                new AlphabetRule("t", "т"),

                new AlphabetRule("U", "У"),
                new AlphabetRule("u", "у"),

                new AlphabetRule("V", "В"),
                new AlphabetRule("v", "в"),
                // ============================================
               // XALQARO LOTIN HARFLARI
              // ============================================

             // Inglizcha W/w — o'zbek kirillida V/v
               new AlphabetRule("W", "В"),
               new AlphabetRule("w", "в"),

                new AlphabetRule("X", "Х"),
                new AlphabetRule("x", "х"),

                new AlphabetRule("Y", "Й"),
                new AlphabetRule("y", "й"),

                new AlphabetRule("Z", "З"),
                new AlphabetRule("z", "з"),

                // ============================================
                // QO'SHIMCHA BELGILAR
                // ============================================

                new AlphabetRule("Ğ", "Ғ"),
                new AlphabetRule("ğ", "ғ"),

                new AlphabetRule("Ö", "Ў"),
                new AlphabetRule("ö", "ў"),

                new AlphabetRule("Ş", "Ш"),
                new AlphabetRule("ş", "ш"),

                new AlphabetRule("Ç", "Ч"),
                new AlphabetRule("ç", "ч")
            };

        /// <summary>
        /// Kirill -> Lotin asosiy qoidalar.
        /// Е/е BU YERDA YO'Q.
        /// Uni Transliterator kontekst bo'yicha hal qiladi.
        /// </summary>
        public static readonly IReadOnlyList<AlphabetRule> CyrillicToLatin =
            new List<AlphabetRule>
            {
                // ============================================
                // ASOSIY HARFLAR
                // ============================================

                new AlphabetRule("А", "A"),
                new AlphabetRule("а", "a"),

                new AlphabetRule("Б", "B"),
                new AlphabetRule("б", "b"),

                new AlphabetRule("В", "V"),
                new AlphabetRule("в", "v"),

                new AlphabetRule("Г", "G"),
                new AlphabetRule("г", "g"),

                new AlphabetRule("Ғ", "G'"),
                new AlphabetRule("ғ", "g'"),

                new AlphabetRule("Д", "D"),
                new AlphabetRule("д", "d"),

                new AlphabetRule("Ё", "Yo"),
                new AlphabetRule("ё", "yo"),

                new AlphabetRule("Ж", "J"),
                new AlphabetRule("ж", "j"),

                new AlphabetRule("З", "Z"),
                new AlphabetRule("з", "z"),

                new AlphabetRule("И", "I"),
                new AlphabetRule("и", "i"),

                new AlphabetRule("Й", "Y"),
                new AlphabetRule("й", "y"),

                new AlphabetRule("К", "K"),
                new AlphabetRule("к", "k"),

                new AlphabetRule("Қ", "Q"),
                new AlphabetRule("қ", "q"),

                new AlphabetRule("Л", "L"),
                new AlphabetRule("л", "l"),

                new AlphabetRule("М", "M"),
                new AlphabetRule("м", "m"),

                new AlphabetRule("Н", "N"),
                new AlphabetRule("н", "n"),

                new AlphabetRule("О", "O"),
                new AlphabetRule("о", "o"),

                new AlphabetRule("Ў", "O'"),
                new AlphabetRule("ў", "o'"),

                new AlphabetRule("П", "P"),
                new AlphabetRule("п", "p"),

                new AlphabetRule("Р", "R"),
                new AlphabetRule("р", "r"),

                new AlphabetRule("С", "S"),
                new AlphabetRule("с", "s"),

                new AlphabetRule("Т", "T"),
                new AlphabetRule("т", "t"),

                new AlphabetRule("У", "U"),
                new AlphabetRule("у", "u"),

                new AlphabetRule("Ф", "F"),
                new AlphabetRule("ф", "f"),

                new AlphabetRule("Х", "X"),
                new AlphabetRule("х", "x"),

                new AlphabetRule("Ҳ", "H"),
                new AlphabetRule("ҳ", "h"),

                new AlphabetRule("Ц", "Ts"),
                new AlphabetRule("ц", "ts"),

                new AlphabetRule("Ч", "Ch"),
                new AlphabetRule("ч", "ch"),

                new AlphabetRule("Ш", "Sh"),
                new AlphabetRule("ш", "sh"),

                new AlphabetRule("Щ", "Shch"),
                new AlphabetRule("щ", "shch"),

                // ============================================
                // MAXSUS KIRILL HARFLAR
                // ============================================

                new AlphabetRule("Э", "E"),
                new AlphabetRule("э", "e"),

                new AlphabetRule("Ю", "Yu"),
                new AlphabetRule("ю", "yu"),

                new AlphabetRule("Я", "Ya"),
                new AlphabetRule("я", "ya"),

                new AlphabetRule("Ы", "I"),
                new AlphabetRule("ы", "i"),

                // ============================================
                // QATTIQ / YUMSHOQ BELGI
                // ============================================

                new AlphabetRule("Ъ", "'"),
                new AlphabetRule("ъ", "'"),

                new AlphabetRule("Ь", ""),
                new AlphabetRule("ь", "")
            };

        /// <summary>
        /// Lotin -> Kirill qoidalarini qaytaradi.
        /// </summary>
        public static IReadOnlyList<AlphabetRule> GetLatinToCyrillicRules()
        {
            return LatinToCyrillic;
        }

        /// <summary>
        /// Kirill -> Lotin qoidalarini qaytaradi.
        /// </summary>
        public static IReadOnlyList<AlphabetRule> GetCyrillicToLatinRules()
        {
            return CyrillicToLatin;
        }
    }
}
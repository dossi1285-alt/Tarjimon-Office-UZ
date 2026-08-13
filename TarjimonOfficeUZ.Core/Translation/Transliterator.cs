using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TarjimonOfficeUZ.Core.Translation
{
    /// <summary>
    /// Tarjimon Office UZ ning asosiy transliteratsiya dvigateli.
    /// Lotin va Kirill yozuvlari o'rtasida ikki tomonlama
    /// transliteratsiyani amalga oshiradi.
    /// </summary>
    public static class Transliterator
    {
        /// <summary>
        /// Matndagi turli apostrof belgilarini yagona ko'rinishga
        /// keltiradi. Matnning boshidagi va oxiridagi bo'shliqlar
        /// o'zgartirilmaydi.
        /// </summary>
        public static string NormalizeText(string text)
        {
            if (text == null)
            {
                return string.Empty;
            }

            text = text.Replace('ʻ', '\'');
            text = text.Replace('ʼ', '\'');
            text = text.Replace('’', '\'');
            text = text.Replace('‘', '\'');
            text = text.Replace('`', '\'');
            text = text.Replace('´', '\'');

            return text;
        }

        /// <summary>
        /// Lotin -> Kirill va mapping.
        /// </summary>
        public static TranslationResult LatinToCyrillicWithMapping(string text)
        {
            text = NormalizeText(text);

            var rules = CurrentAlphabet.GetLatinToCyrillicRules();

            return ApplyRulesWithMapping(
                text,
                rules,
                TranslationDirection.LatinToCyrillic);
        }

        /// <summary>
        /// Kirill -> Lotin va mapping.
        /// </summary>
        public static TranslationResult CyrillicToLatinWithMapping(string text)
        {
            text = NormalizeText(text);

            var rules = CurrentAlphabet.GetCyrillicToLatinRules();

            return ApplyRulesWithMapping(
                text,
                rules,
                TranslationDirection.CyrillicToLatin);
        }

        /// <summary>
        /// Lotin matnini Kirill yozuviga o'giradi.
        /// </summary>
        public static string LatinToCyrillic(string text)
        {
            return LatinToCyrillicWithMapping(text).Text;
        }

        /// <summary>
        /// Kirill matnini Lotin yozuviga o'giradi.
        /// </summary>
        public static string CyrillicToLatin(string text)
        {
            return CyrillicToLatinWithMapping(text).Text;
        }

        /// <summary>
        /// Asosiy transliteratsiya algoritmi.
        /// Maxsus kontekst qoidalari avval tekshiriladi,
        /// keyin CurrentAlphabet dagi umumiy qoidalar ishlaydi.
        /// </summary>
        private static TranslationResult ApplyRulesWithMapping(
            string text,
            IReadOnlyList<AlphabetRule> rules,
            TranslationDirection direction)
        {
            var result = new TranslationResult();

            if (string.IsNullOrEmpty(text))
            {
                return result;
            }

            var orderedRules = rules
                .OrderByDescending(r => r.Source.Length)
                .ToList();

            var builder = new StringBuilder();

            int i = 0;

            while (i < text.Length)
            {
                AlphabetRule specialRule;
                // ====================================================
                // W/w YOKI YOLG'IZ C/c QATNASHGAN BUTUN LOTIN SO'ZINI
                // HIMOYALASH (xalqaro so'zlar)
                //
                // Masalan:
                // Windows  -> Windows
                // windows  -> windows
                // Web      -> Web
                // World    -> World
                // Cisco    -> Cisco
                // century  -> century
                //
                // "ch" digrafi (cho'l, choy kabi) bu qoidaga tushmaydi —
                // u alohida CH/Ch/ch qoidasi bilan "ч" ga o'giriladi.
                //
                // W/w yoki yolg'iz C/c bo'lmagan so'zlar oddiy qoidalar
                // bilan tarjima qilinadi.
                // ====================================================

                if (direction == TranslationDirection.LatinToCyrillic &&
                    TryGetProtectedLatinWord(
                        text,
                        i,
                        out string protectedWord,
                        out int protectedLength))
                {
                    int protectedStart = builder.Length;

                    builder.Append(protectedWord);

                    for (int k = 0; k < protectedWord.Length; k++)
                    {
                        var map = new CharacterMap
                        {
                            NewIndex = protectedStart + k
                        };

                        map.SourceIndexes.Add(i + k);

                        result.Mapping.Add(map);
                    }

                    i += protectedLength;

                    continue;
                }

                // ============================================
                // 1. KONTEKSTLI MAXSUS QOIDA
                // ============================================

                if (TryGetSpecialRule(
                    text,
                    i,
                    direction,
                    out specialRule))
                {
                    AppendRuleResult(
                        result,
                        builder,
                        specialRule,
                        i);

                    i += specialRule.Source.Length;
                    continue;
                }

                // ============================================
                // 2. UMUMIY QOIDALAR
                // ============================================

                bool matched = false;

                foreach (var rule in orderedRules)
                {
                    if (i + rule.Source.Length > text.Length)
                    {
                        continue;
                    }

                    if (!TextStartsWith(
                        text,
                        i,
                        rule.Source))
                    {
                        continue;
                    }

                    AppendRuleResult(
                        result,
                        builder,
                        rule,
                        i);

                    i += rule.Source.Length;
                    matched = true;
                    break;
                }

                if (matched)
                {
                    continue;
                }

                // ============================================
                // 3. QOIDA TOPILMAGAN BELGI
                // O'Z HOLICHA QOLADI
                // ============================================

                int currentIndex = builder.Length;

                builder.Append(text[i]);

                var single = new CharacterMap
                {
                    NewIndex = currentIndex
                };

                single.SourceIndexes.Add(i);

                result.Mapping.Add(single);

                i++;
            }

            result.Text = builder.ToString();

            return result;
        }

        /// <summary>
        /// Maxsus kontekstli qoidalarni aniqlaydi.
        /// </summary>
        private static bool TryGetSpecialRule(
            string text,
            int index,
            TranslationDirection direction,
            out AlphabetRule rule)
        {
            rule = null;

            if (direction == TranslationDirection.LatinToCyrillic)
            {
                return TryGetLatinToCyrillicSpecialRule(
                    text,
                    index,
                    out rule);
            }

            if (direction == TranslationDirection.CyrillicToLatin)
            {
                return TryGetCyrillicToLatinSpecialRule(
                    text,
                    index,
                    out rule);
            }

            return false;
        }

        // ====================================================
        // LOTIN -> KIRILL MAXSUS QOIDALAR
        // ====================================================

        private static bool TryGetLatinToCyrillicSpecialRule(
            string text,
            int index,
            out AlphabetRule rule)
        {
            rule = null;

            if (index >= text.Length)
            {
                return false;
            }

            char current = text[index];
            
            // ------------------------------------------------
            // E' / e'
            //
            // e'lon  -> эълон
            // E'lon  -> Эъ...
            // ------------------------------------------------

            if (current == 'E' || current == 'e')
            {
                if (index + 1 < text.Length &&
                    text[index + 1] == '\'')
                {
                    if (current == 'E')
                    {
                        rule = new AlphabetRule("E'", "Эъ");
                    }
                    else
                    {
                        rule = new AlphabetRule("e'", "эъ");
                    }

                    return true;
                }
        }

            // ------------------------------------------------
            // APOSTROF
            //
            // ma'lumot -> маълумот
            // san'at   -> санъат
            //
            // O' / G' bu yerga tushmaydi, chunki ular
            // CurrentAlphabet dagi uzunroq qoidalar bilan
            // alohida ishlanadi.
            // ------------------------------------------------

            if (current == '\'')
            {
                rule = new AlphabetRule("'", "ъ");
                return true;
            }

            // ------------------------------------------------
            // E/e
            //
            // So'z boshida:
            // eshik -> эшик
            // erkak -> эркак
            //
            // So'z ichida:
            // ber -> бер
            // kel -> кел
            //
            // ------------------------------------------------

            if (current == 'E' || current == 'e')
            {
                

                // --------------------------------------------
                // E' / e' oldingi maxsus qoida orqali
                // allaqachon ushlanadi.
                //
                // Oddiy E/e:
                // eshik -> эшик
                // erkak -> эркак
                // ber -> бер
                // kel -> кел
                // --------------------------------------------

                bool wordStart = IsLatinWordStart(text, index);

                if (wordStart)
                {
                    rule = current == 'E'
                        ? new AlphabetRule("E", "Э")
                        : new AlphabetRule("e", "э");

                    return true;
                }

                rule = current == 'E'
                    ? new AlphabetRule("E", "Е")
                    : new AlphabetRule("e", "е");

                return true;
            }
            return false;
        }



        // ====================================================
        // KIRILL -> LOTIN MAXSUS QOIDALAR
        // ====================================================

        private static bool TryGetCyrillicToLatinSpecialRule(
            string text,
            int index,
            out AlphabetRule rule)
        {
            rule = null;

            if (index >= text.Length)
            {
                return false;
            }

            char current = text[index];

            // ====================================================
            // Е / е
            //
            // So'z boshida yoki unlidan keyin:
            //
            // Ер      -> Yer
            // Европа  -> Yevropa
            //
            // Undoshdan keyin:
            //
            // бер -> ber
            // кел -> kel
            // ====================================================

            if (current == 'Е' || current == 'е')
            {
                bool useYe =
                    ShouldUseYeForCyrillicE(
                        text,
                        index);

                if (useYe)
                {
                    rule = current == 'Е'
                        ? new AlphabetRule("Е", "Ye")
                        : new AlphabetRule("е", "ye");

                    return true;
                }

                rule = current == 'Е'
                    ? new AlphabetRule("Е", "E")
                    : new AlphabetRule("е", "e");

                return true;
            }

            return false;
        }

        // ====================================================
        // LOTIN SO'ZIDA W/w YOKI YOLG'IZ C/c BORLIGINI TEKSHIRISH
        //
        // Agar butun so'z tarkibida W yoki w bo'lsa,
        // yoki "ch" digrafiga kirmagan C/c bo'lsa,
        // o'sha so'z tarjima qilinmaydi.
        //
        // Windows -> Windows
        // windows -> windows
        // Web     -> Web
        // web     -> web
        // World   -> World
        // world   -> world
        // Cisco   -> Cisco
        // century -> century
        //
        // ch/Ch/CH digrafi bu yerga kirmaydi:
        // cho'l -> cho'l so'zidagi "ch" oddiy CH qoidasi
        // bilan "ч" ga o'giriladi, so'z himoyalanmaydi.
        //
        // W/w va yolg'iz C/c bo'lmagan so'zlar oddiy alfabet
        // qoidalari bilan tarjima qilinadi.
        // ====================================================

        private static bool TryGetProtectedLatinWord(
            string text,
            int index,
            out string word,
            out int length)
        {
            word = null;
            length = 0;

            if (string.IsNullOrEmpty(text))
                return false;

            if (index < 0 || index >= text.Length)
                return false;

            // So'zning o'rtasidan tekshirishni boshlamaymiz.
            if (index > 0 &&
                char.IsLetterOrDigit(text[index - 1]))
            {
                return false;
            }

            int end = index;

            while (end < text.Length &&
                   char.IsLetterOrDigit(text[end]))
            {
                end++;
            }

            if (end <= index)
                return false;

            string candidate =
                text.Substring(index, end - index);

            if (ContainsW(candidate) ||
                ContainsStandaloneC(candidate))
            {
                word = candidate;
                length = candidate.Length;

                return true;
            }

            return false;
        }

        // ====================================================
        // SO'ZDA W/w BORLIGINI TEKSHIRISH
        // ====================================================

        private static bool ContainsW(string word)
        {
            return word.IndexOf('W') >= 0 ||
                   word.IndexOf('w') >= 0;
        }

        // ====================================================
        // SO'ZDA "CH" DIGRAFIGA KIRMAGAN YOLG'IZ C/c BORLIGINI
        // TEKSHIRISH
        //
        // cho'l  -> C dan keyin H keladi -> ch digraf -> e'tiborsiz
        // Cisco  -> C dan keyin I keladi -> yolg'iz C -> himoyalanadi
        // disc   -> so'z oxiridagi C dan keyin harf yo'q -> yolg'iz C
        // ====================================================

        private static bool ContainsStandaloneC(string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];

                if (c != 'C' && c != 'c')
                {
                    continue;
                }

                bool isChDigraph =
                    i + 1 < word.Length &&
                    (word[i + 1] == 'H' || word[i + 1] == 'h');

                if (!isChDigraph)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsLatinWordStart(
            string text,
            int index)
        {
            if (index <= 0)
            {
                return true;
            }

            int p = index - 1;

            // Apostrof O' / G' kabi kombinatsiyaning qismi
            // bo'lsa, undan oldingi harfni ko'ramiz.
            while (p >= 0 && text[p] == '\'')
            {
                p--;
            }

            if (p < 0)
            {
                return true;
            }

            char previous = text[p];

            // Oldingi belgi harf yoki raqam bo'lmasa,
            // yangi so'z boshlangan deb hisoblaymiz.
            return !char.IsLetterOrDigit(previous);
        }

        // ====================================================
        // KIRILL Е/е UCHUN Ye YOKI E TANLASH
        // ====================================================

        private static bool ShouldUseYeForCyrillicE(
            string text,
            int index)
        {
            if (index <= 0)
            {
                return true;
            }

            char previous = text[index - 1];

            // So'z boshidan keyingi pozitsiya.
            // Oldingi belgi harf/raqam emas.
            if (!char.IsLetterOrDigit(previous))
            {
                return true;
            }

            // Е unlidan keyin kelsa -> Ye.
            //
            // аёл -> ayol emas bu holatda "ё" bo'ladi,
            // lekin umumiy Е qoidasi:
            // vowel + Е -> Ye.
            //
            // Европа -> Yevropa.
            if (IsCyrillicVowel(previous))
            {
                return true;
            }

            return false;
        }

        // ====================================================
        // KIRILL UNLILARI
        // ====================================================

        private static bool IsCyrillicVowel(char c)
        {
            switch (c)
            {
                case 'А':
                case 'а':
                case 'Е':
                case 'е':
                case 'Ё':
                case 'ё':
                case 'И':
                case 'и':
                case 'О':
                case 'о':
                case 'У':
                case 'у':
                case 'Ў':
                case 'ў':
                case 'Ы':
                case 'ы':
                case 'Э':
                case 'э':
                case 'Ю':
                case 'ю':
                case 'Я':
                case 'я':
                    return true;

                default:
                    return false;
            }
        }

        // ====================================================
        // MATN BOSHLANISHINI TEKSHIRISH
        // ====================================================

        private static bool TextStartsWith(
            string text,
            int index,
            string source)
        {
            if (index + source.Length > text.Length)
            {
                return false;
            }

            for (int i = 0; i < source.Length; i++)
            {
                if (text[index + i] != source[i])
                {
                    return false;
                }
            }

            return true;
        }

        // ====================================================
        // MAPPING YARATISH
        // ====================================================

        private static void AppendRuleResult(
            TranslationResult result,
            StringBuilder builder,
            AlphabetRule rule,
            int sourceIndex)
        {
            int newIndex = builder.Length;

            builder.Append(rule.Target);

            for (int k = 0; k < rule.Target.Length; k++)
            {
                var map = new CharacterMap
                {
                    NewIndex = newIndex + k
                };

                for (int s = 0; s < rule.Source.Length; s++)
                {
                    map.SourceIndexes.Add(sourceIndex + s);
                }

                result.Mapping.Add(map);
            }
        }
    }
}
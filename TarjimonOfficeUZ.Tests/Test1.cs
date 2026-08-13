using TarjimonOfficeUZ.Core.Translation;

namespace TarjimonOfficeUZ.Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void LatinToCyrillic_WordWithW_RemainsUnchanged()
        {
            Assert.AreEqual("Windows windows Web web World world", 
                Transliterator.LatinToCyrillic("Windows windows Web web World world"));
        }

        [TestMethod]
        public void LatinToCyrillic_WordWithC_RemainsUnchanged_V1()
        {
            Assert.AreEqual("Cisco cisco Excel Office Computer", 
                Transliterator.LatinToCyrillic("Cisco cisco Excel Office Computer"));
        }

        [TestMethod]
        public void LatinToCyrillic_WAndC_InternationalWordsRemainUnchangedWithPunctuation()
        {
            Assert.AreEqual("Windows2, Web! Cisco, Office-2026 Computer!", 
                Transliterator.LatinToCyrillic("Windows2, Web! Cisco, Office-2026 Computer!"));
        }

        [TestMethod]
        public void LatinToCyrillic_WordWithW_PreservesPunctuationAndDigits()
        {
            Assert.AreEqual("Windows2, Web! World-Боок", 
                Transliterator.LatinToCyrillic("Windows2, Web! World-Book"));
        }

        [TestMethod]
        public void LatinToCyrillic_ChDigraph_IsStillTransliterated()
        {
            Assert.AreEqual("чой Чирчиқ Чин", 
                Transliterator.LatinToCyrillic("choy Chirchiq Chin"));
        }

        [TestMethod]
        public void LatinToCyrillic_EContext_IsHandled()
        {
            Assert.AreEqual("эълон Эълон эшик эркак бер кел", 
                Transliterator.LatinToCyrillic("e'lon E'lon eshik erkak ber kel"));
        }

        [TestMethod]
        public void CyrillicToLatin_EContext_IsHandled()
        {
            Assert.AreEqual("Yer Yevropa ber kel", 
                Transliterator.CyrillicToLatin("Ер Европа бер кел"));
        }

        [TestMethod]
        public void LatinToCyrillic_BasicUzbekLetters_AreConverted()
        {
            Assert.AreEqual("А а Б б Г г Ғ ғ Ҳ ҳ Қ қ Х х", 
                Transliterator.LatinToCyrillic("A a B b G g G' g' H h Q q X x"));
        }

        [TestMethod]
        public void CyrillicToLatin_BasicUzbekLetters_AreConverted()
        {
            Assert.AreEqual("A a B b G g G' g' H h Q q X x", 
                Transliterator.CyrillicToLatin("А а Б б Г г Ғ ғ Ҳ ҳ Қ қ Х х"));
        }

        [TestMethod]
        public void LatinToCyrillic_NullAndEmpty_AreSafe()
        {
            Assert.AreEqual(string.Empty, Transliterator.LatinToCyrillic(null));
            Assert.AreEqual(string.Empty, Transliterator.LatinToCyrillic(string.Empty));
        }
    }
}

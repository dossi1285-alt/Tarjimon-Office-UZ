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
        public void LatinToCyrillic_WordWithW_PreservesPunctuationAndDigits()
        {
            Assert.AreEqual("Windows2, Web! World-Office", 
                Transliterator.LatinToCyrillic("Windows2, Web! World-Office"));
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

using NUnit.Framework;
using Eleks.Demo.Common.Extensions;

namespace Common.Tests
{
    [TestFixture]
    public class StringExtension_Capitalize_Tests
    {
        [Test]
        public void WhenNull_ReturnsNull()
        {
            string s = null;
            Assert.AreEqual(null, s.Capitalize());
        }

        [Test]
        public void WhenEmpty_ReturnsEmpty()
        {
            Assert.AreEqual("", "".Capitalize());
        }

        [Test]
        public void WhenSingleLetter_UpperCaseThatLetter()
        {
            Assert.AreEqual("A", "a".Capitalize());
        }

        [Test]
        public void ForAnyString_UpperFirstLetterAndLowerAllOthers()
        {
            Assert.AreEqual("Ab", "aB".Capitalize());
        }

        [Test]
        public void SeveralSpaceSeparatedWords()
        {
            Assert.AreEqual("Hello world!", "hello WORLD!".Capitalize());
        }
    }
}
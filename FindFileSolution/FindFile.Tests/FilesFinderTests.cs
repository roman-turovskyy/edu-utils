using System;
using Eleks.Demo;
using NUnit.Framework;

namespace FindFile.Tests
{
    [TestFixture]
    public class FilesFinderTests
    {
        [Test]
        public void SearchWithSameNames_PassingNullString_ThrowsArgumentNullException()
        {
            var filesFinder = new FilesFinder();
            Assert.Throws<ArgumentNullException>(() => filesFinder.SearchWithSameNames((string)null));
        }

        [Test]
        public void SearchWithSameNames_PassingNullStringArray_ThrowsArgumentNullException()
        {
            var filesFinder = new FilesFinder();
            Assert.Throws<ArgumentNullException>(() => filesFinder.SearchWithSameNames((string[])null));
        }
    }
}

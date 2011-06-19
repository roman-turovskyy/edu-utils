using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Eleks.Demo.UnitTests
{
    [TestFixture]
    public class FilesFinder_SearchWithSameName_Tests
    {
        [Test]
        public void PassingNull_ThrowsArgumentNullException()
        {
            var filesFinder = new FilesFinder(new DirServiceStub());
            Assert.Throws<ArgumentNullException>(() => filesFinder.SearchWithSameNames(null));
        }

        [Test]
        public void PassingNonExistingDirectory_RegistersError__CustomStub()
        {
            // Arrange
            var dirService = new DirServiceStub();
            dirService.DirectoryExistsResult = false;
            var filesFinder = new FilesFinder(dirService);
            // Act
            filesFinder.SearchWithSameNames("NonExisting");
            // Assert
            Assert.AreEqual(1, filesFinder.GetLastErrors().Count, "Non existing directory error expected");
        }

        [Test]
        public void PassingNonExistingDirectory_RegistersError__MoqStub()
        {
            // Arrange
            var dirServiceStub = new Moq.Mock<IDirService>();
            dirServiceStub.Setup(s => s.DirectoryExists("NonExisting")).Returns(false);
            var filesFinder = new FilesFinder(dirServiceStub.Object);
            // Act
            filesFinder.SearchWithSameNames("NonExisting");
            // Assert
            Assert.AreEqual(1, filesFinder.GetLastErrors().Count, "Non existing directory error expected");
        }

        [Test]
        public void InEmptyDir_NoResults__CustomStub()
        {
            // Arrange
            var dirService = new DirServiceStub();
            dirService.DirectoryExistsResult = true;
            dirService.GetFilesResult = new string[0];
            dirService.GetDirectoriesResult = new string[0];
            var filesFinder = new FilesFinder(dirService);
            // Act
            var result = filesFinder.SearchWithSameNames("Empty");
            // Assert
            Assert.AreEqual(0, result.Count, "No files expected in empty directory");
        }

        [Test]
        public void InEmptyDir_NoResults__MoqStub()
        {
            // Arrange
            var dirServiceStub = new Moq.Mock<IDirService>();
            dirServiceStub.Setup(s => s.DirectoryExists("Empty")).Returns(true);
            var filesFinder = new FilesFinder(dirServiceStub.Object);
            // Act
            var result = filesFinder.SearchWithSameNames("Empty");
            // Assert
            Assert.AreEqual(0, result.Count, "No files expected in empty directory");
        }

        [Test]
        public void InDirWithDistinctFiles_NoResults()
        {
            // Arrange
            var dirServiceStub = new Moq.Mock<IDirService>();
            dirServiceStub.Setup(s => s.DirectoryExists("A")).Returns(true);
            dirServiceStub.Setup(s => s.GetFiles("A")).Returns(new[] {@"A\1.txt", @"A\2.txt"});
            var filesFinder = new FilesFinder(dirServiceStub.Object);
            // Act
            var result = filesFinder.SearchWithSameNames("A");
            // Assert
            Assert.AreEqual(0, result.Count, "No files expected");
        }

        [Test]
        public void InDirWith2SameFiles_1Result()
        {
            // Arrange
            var dirServiceStub = new Moq.Mock<IDirService>();
            dirServiceStub.Setup(s => s.DirectoryExists("A")).Returns(true);
            dirServiceStub.Setup(s => s.GetDirectories("A")).Returns(new[] {@"A\B"});
            dirServiceStub.Setup(s => s.GetFiles("A")).Returns(new[] { @"A\1.txt", @"A\2.txt" });
            dirServiceStub.Setup(s => s.DirectoryExists(@"A\B")).Returns(true);
            dirServiceStub.Setup(s => s.GetFiles(@"A\B")).Returns(new[] { @"A\B\1.txt" });
            var filesFinder = new FilesFinder(dirServiceStub.Object);
            // Act
            var result = filesFinder.SearchWithSameNames("A");
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("1.txt", result[0].FileName);
            Assert.AreEqual(2, result[0].Count);
        }

        [Test]
        public void InDirWith2SameFilesButDifferentCase_1Result()
        {
            // Arrange
            var dirService = new DirServiceSmartStub(@"
A\x.txt
A\y.txt
A\B\X.txt
");
            var filesFinder = new FilesFinder(dirService);
            // Act
            var result = filesFinder.SearchWithSameNames("A");
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("x.txt", result[0].FileName);
            Assert.AreEqual(2, result[0].Count);
        }

        [Test]
        public void InDirWith2Plus2SameFiles_2Results()
        {
            // Arrange
            var dirService = new DirServiceSmartStub(@"
a\1.txt
a\2.txt
a\b\1.txt
a\b\2.txt
a\b\c\2.txt
");
            var filesFinder = new FilesFinder(dirService);
            // Act
            var result = filesFinder.SearchWithSameNames("a");
            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1.txt", result[0].FileName);
            Assert.AreEqual(2, result[0].Count);

            Assert.AreEqual("2.txt", result[1].FileName);
            Assert.AreEqual(3, result[1].Count);
        }

        [Test]
        public void UnauthorizedAccessExceptionThrown_ErrorRegistered()
        {
            // Arrange
            var dirServiceStub = new Moq.Mock<IDirService>();
            dirServiceStub.Setup(s => s.DirectoryExists(Moq.It.IsAny<string>())).Returns(true);
            dirServiceStub.Setup(s => s.GetDirectories("a")).Throws<UnauthorizedAccessException>();
            var filesFinder = new FilesFinder(dirServiceStub.Object);
            // Act
            filesFinder.SearchWithSameNames("a");
            // Assert
            Assert.AreEqual(1, filesFinder.GetLastErrors().Count);
        }
    }

    internal class DirServiceStub : IDirService
    {
        public bool DirectoryExistsResult;
        public string[] GetDirectoriesResult;
        public string[] GetFilesResult;

        public string[] GetFiles(string path)
        {
            return GetFilesResult;
        }

        public bool DirectoryExists(string path)
        {
            return DirectoryExistsResult;
        }

        public string[] GetDirectories(string path)
        {
            return GetDirectoriesResult;
        }
    }

    [TestFixture]
    public class DirServiceSmartStubTests
    {
        [Test]
        public void DirectoryExists()
        {
            var stub = new DirServiceSmartStub(@"
A\B\1
");
            Assert.IsTrue(stub.DirectoryExists("A"));
            Assert.IsTrue(stub.DirectoryExists(@"A\B"));
            Assert.IsTrue(stub.DirectoryExists(@"a\b"), "Must be case insensitive");
            Assert.IsFalse(stub.DirectoryExists(@"A\B\1"));
            Assert.IsFalse(stub.DirectoryExists(@"B"));
        }

        [Test]
        public void GetDirectories()
        {
            var stub = new DirServiceSmartStub(@"
A\B\1
a\a\z\
");
            TAssert.ArraysEqual(stub.GetDirectories(@"a"), new[] { @"A\B", @"a\a" });
            TAssert.ArraysEqual(stub.GetDirectories(@"A\B"), new string[0]);
            TAssert.ArraysEqual(stub.GetDirectories(@"a\a"), new[] { @"a\a\z" });
            TAssert.ArraysEqual(stub.GetDirectories(@"a\a\z"), new string[0]);

            TAssert.ArraysEqual(stub.GetDirectories(@"foo"), new string[0]);
            TAssert.ArraysEqual(stub.GetDirectories(@"B"), new string[0]);
        }

        [Test]
        public void GetFiles()
        {
            var stub = new DirServiceSmartStub(@"
A\B\1
A\B\2
a\a\z\
");
            TAssert.ArraysEqual(stub.GetFiles(@"a"), new string[0]);
            TAssert.ArraysEqual(stub.GetFiles(@"a\b"), new[] { "1", "2" });
            TAssert.ArraysEqual(stub.GetFiles(@"a\a\z"), new string[0]);
        }
    }
    
    public class DirServiceSmartStub : IDirService
    {
        private string[] m_Lines;

        public DirServiceSmartStub(string data)
        {
            if (data == null) throw new ArgumentNullException("data");
            m_Lines = data.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetFiles(string path)
        {
            path = path.EnsureEndsWith('\\');
            return m_Lines.Where(
                line => line.StartsWith(path, StringComparison.InvariantCultureIgnoreCase)
                && line.LastIndexOf('\\') == path.LastIndexOf('\\')
                && line.Length > path.Length)
                .ToArray();
        }

        public bool DirectoryExists(string path)
        {
            path = path.EnsureEndsWith('\\');
            return m_Lines.FirstOrDefault(
                line => line.StartsWith(path, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        public string[] GetDirectories(string path)
        {
            path = path.EnsureEndsWith('\\');
            return m_Lines.Where(
                line => line.StartsWith(path, StringComparison.InvariantCultureIgnoreCase)
                && line.LastIndexOf('\\') > path.LastIndexOf('\\'))
                .Select(line => line.Substring(0, line.IndexOf('\\', path.Length)))
                .ToArray();
        }
    }

    public static class StringExtensions
    {
        public static string EnsureEndsWith(this string str, char c)
        {
            return str.EndsWith(c.ToString(), StringComparison.Ordinal)
                       ? str
                       : str + c;
        }
    }

}
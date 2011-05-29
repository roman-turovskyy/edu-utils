using System;
using System.IO;
using Eleks.Demo;
using NUnit.Framework;

namespace FindFile.Tests
{
    [TestFixture]
    public class FilesFinderTests
    {
        private string RootTestDir
        {
            get
            {
                // D:\apps\blah\FindFiles.Tests\bin\Debug -> D:\apps\blah\FindFiles.Tests\bin\Debug\..\..\TestDir
                string path = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDir");
                if (!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException(path);
                }
                // D:\apps\blah\FindFiles.Tests\bin\Debug\..\..\TestDir -> D:\apps\blah\FindFiles.Tests\TestDir
                return Path.GetFullPath(path);
            }
        }

        [Test]
        public void SearchWithSameNames_PassingNull_ThrowsArgumentNullException()
        {
            var filesFinder = new FilesFinder();
            Assert.Throws<ArgumentNullException>(() => filesFinder.SearchWithSameNames(null));
        }

        [Test]
        public void SearchWithSameNames_PassingNonExistingDirectory_RegistersError()
        {
            // Arrange
            var filesFinder = new FilesFinder();
            // Act
            string nonExistingDir = Path.Combine(RootTestDir, "NonExisting");
            filesFinder.SearchWithSameNames(nonExistingDir);
            // Assert
            Assert.AreEqual(1, filesFinder.GetLastErrors().Count, "Non existing directory error expected");
        }

        [Test]
        public void SearchWithSameNames_InEmptyDir_NoResults()
        {
            // Arrange
            var filesFinder = new FilesFinder();
            // Act
            var result = filesFinder.SearchWithSameNames(Path.Combine(RootTestDir, "Empty"));
            // Assert
            Assert.AreEqual(0, result.Count, "No files expected in empty directory");
        }

        [Test]
        public void SearchWithSameNames_InDirWithDistinctFiles_NoResults()
        {
            // Arrange
            var filesFinder = new FilesFinder();
            // Act
            var result = filesFinder.SearchWithSameNames(Path.Combine(RootTestDir, "NoFilesWithSameName"));
            // Assert
            Assert.AreEqual(0, result.Count, "No files expected");
        }

        [Test]
        public void SearchWithSameNames_InDirWith2SameFiles_1Result()
        {
            // Arrange
            var filesFinder = new FilesFinder();
            // Act
            var result = filesFinder.SearchWithSameNames(Path.Combine(RootTestDir, "HasFilesWithSameName"));
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a.txt", result[0].FileName);
            Assert.AreEqual(2, result[0].Count);
        }

        [Test]
        public void SearchWithSameNames_InDirWith2SameFilesButDiffNameCase_1Result()
        {
            // Arrange
            var filesFinder = new FilesFinder();
            // Act
            var result = filesFinder.SearchWithSameNames(Path.Combine(RootTestDir, "HasFilesWithSameNameDiffCase"));
            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a.txt", result[0].FileName.ToLowerInvariant());
            Assert.AreEqual(2, result[0].Count);
        }
    }
}

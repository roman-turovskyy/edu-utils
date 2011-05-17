using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace findfile
{
    internal class FileSearcher
    {
        private readonly FilesCountDictionary m_FilesList = new FilesCountDictionary();

        public IEnumerable<FileAndCount> SearchWithSameNames(string[] directories)
        {
            if (directories == null) throw new ArgumentNullException("directories");
            m_FilesList.Clear();
            foreach (string dir in directories)
            {
                SearchRec(dir);
            }

            return WithSameNames();
        }

        private IEnumerable<FileAndCount> WithSameNames()
        {
            return from pair in m_FilesList
                   where pair.Value > 1
                   select new FileAndCount(pair.Key, pair.Value);
        }

        private void SearchRec(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            foreach (var fullFileName in files)
            {
                string fileName = Path.GetFileName(fullFileName);
                m_FilesList.AddFile(fileName);
            }

            string[] subdirs = Directory.GetDirectories(dir);
            foreach (var sub in subdirs)
            {
                SearchRec(sub);
            }
        }
    }

    internal struct FileAndCount
    {
        public string FileName;
        public int Count;

        public FileAndCount(string fileName, int count)
        {
            FileName = fileName;
            Count = count;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace findfile
{
    internal class FileSearcher
    {
        private readonly ArgumentsParser m_Parser;
        private readonly FilesCountDictionary m_FilesList = new FilesCountDictionary();

        public FileSearcher(ArgumentsParser parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (parser.HasInvalidArgs) throw new ArgumentNullException(parser.ErrorMessage);
            if (parser.CompareSize)
            {
                throw new NotImplementedException("parser.CompareSize not implemented yet");
            }
            m_Parser = parser;
        }

        public IEnumerable<FileAndCount> SearchWithSameNames()
        {
            m_FilesList.Clear();
            foreach (string dir in m_Parser.Dirs)
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
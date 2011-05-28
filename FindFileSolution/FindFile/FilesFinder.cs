using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eleks.Demo
{
    public class FilesFinder
    {
        private readonly Dictionary<string, int> m_FilesCountMap =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> m_AlreadyVisitedDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// This method does not traverse same directories twice even if user passes multiple same directories
        /// </summary>
        public IEnumerable<FileAndCount> SearchWithSameNames(IEnumerable<string> inDirectories)
        {
            if (inDirectories == null) throw new ArgumentNullException("inDirectories");
            m_FilesCountMap.Clear();
            m_AlreadyVisitedDirs.Clear();

            foreach (string dir in inDirectories)
            {
                SearchRec(dir);
            }

            return GetWithSameNames();
        }

        public IEnumerable<FileAndCount> SearchWithSameNames(string inDirectory)
        {
            if (inDirectory == null) throw new ArgumentNullException("inDirectory");
            return SearchWithSameNames(new[] { inDirectory });
        }


        private IEnumerable<FileAndCount> GetWithSameNames()
        {
            return m_FilesCountMap
                .Where(pair => pair.Value > 1)
                .Select(pair => new FileAndCount(pair.Key, pair.Value));
        }

        private void SearchRec(string dir)
        {
            if (!m_AlreadyVisitedDirs.Add(dir))
            {
                // such directory was already visited
                return;
            }

            string[] files = Directory.GetFiles(dir);
            foreach (var fullFileName in files)
            {
                string fileName = Path.GetFileName(fullFileName);
                AddFile(fileName);
            }

            string[] subdirs = Directory.GetDirectories(dir);
            foreach (var sub in subdirs)
            {
                SearchRec(sub);
            }
        }

        private void AddFile(string fileName)
        {
            int count;
            if(m_FilesCountMap.TryGetValue(fileName, out count))
            {
                m_FilesCountMap[fileName] = count + 1;
            }
            else
            {
                m_FilesCountMap.Add(fileName, 1);
            }
        }
    }

    public struct FileAndCount
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
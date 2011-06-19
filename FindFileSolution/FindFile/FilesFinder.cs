using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Eleks.Demo
{
    public class FilesFinder
    {
        private readonly Dictionary<string, int> m_FilesCountMap =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> m_AlreadyVisitedDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private readonly List<string> m_Errors = new List<string>();

        private IDirService m_DirService;

        public FilesFinder(IDirService dirService)
        {
            if (dirService == null) throw new ArgumentNullException("dirService");
            m_DirService = dirService;
        }

        public FilesFinder() : this(new OsDirService())
        {
        }

        /// <summary>
        /// This method does not traverse same directories twice even if user passes multiple same directories
        /// </summary>
        public List<FileAndCount> SearchWithSameNames(params string[] inDirectories)
        {
            if (inDirectories == null) throw new ArgumentNullException("inDirectories");

            m_FilesCountMap.Clear();
            m_AlreadyVisitedDirs.Clear();
            m_Errors.Clear();

            foreach (string dir in inDirectories)
            {
                SearchRec(dir);
            }

            return GetWithSameNames();
        }

        public ReadOnlyCollection<string> GetLastErrors()
        {
            return m_Errors.AsReadOnly();
        }

        private List<FileAndCount> GetWithSameNames()
        {
            var result = new List<FileAndCount>();
            foreach (var pair in m_FilesCountMap)
            {
                if (pair.Value > 1) result.Add(new FileAndCount(pair.Key, pair.Value));
            }
            return result;
        }

        private void SearchRec(string dir)
        {
            if (!m_AlreadyVisitedDirs.Add(dir))
            {
                // such directory was already visited. Skip it with subdirs
                return;
            }

            if (!m_DirService.DirectoryExists(dir))
            {
                RegisterError(string.Format("Directory '{0}' not found", dir));
                return;
            }

            string[] files = GetFilesSafe(dir);
            foreach (var fullFileName in files)
            {
                string fileName = Path.GetFileName(fullFileName);
                AddFile(fileName);
            }

            string[] subdirs = GetSubDirectoriesSafe(dir);
            foreach (var sub in subdirs)
            {
                SearchRec(sub);
            }
        }

        private string[] GetFilesSafe(string dir)
        {
            var files = new string[0];
            try
            {
                files = m_DirService.GetFiles(dir);
            }
            catch(DirectoryNotFoundException e)
            {
                RegisterError(e);
            }
            catch(UnauthorizedAccessException e)
            {
                RegisterError(e);
            }
            catch (IOException e)
            {
                RegisterError(e);
            }
            return files;
        }

        private void RegisterError(Exception ex)
        {
            RegisterError(ex.Message);
        }

        private void RegisterError(string errMsg)
        {
            m_Errors.Add(errMsg);
        }

        private string[] GetSubDirectoriesSafe(string dir)
        {
            var subDirs = new string[0];
            try
            {
                subDirs = m_DirService.GetDirectories(dir);
            }
            catch (DirectoryNotFoundException e)
            {
                RegisterError(e);
            }
            catch (UnauthorizedAccessException e)
            {
                RegisterError(e);
            }
            catch (IOException e)
            {
                RegisterError(e);
            }
            return subDirs;
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
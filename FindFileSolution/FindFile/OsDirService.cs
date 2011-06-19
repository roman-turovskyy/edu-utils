using System;
using System.IO;

namespace Eleks.Demo
{
    public class OsDirService : IDirService
    {
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
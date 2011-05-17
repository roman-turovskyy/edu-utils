using System;
using System.Collections.Generic;

namespace findfile
{
    internal class FilesCountDictionary : Dictionary<string, int>
    {
        public void AddFile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            fileName = fileName.ToLowerInvariant();
            int count;
            if (TryGetValue(fileName, out count))
            {
                this[fileName] = count + 1;
            }
            else
            {
                this[fileName] = 1;
            }
        }
    }
}

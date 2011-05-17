using System;
using System.IO;
using System.Linq;

namespace findfile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            if (!ValidateArguments(args))
            {
                return;
            }

            try
            {
                var searcher = new FileSearcher();
                var files = searcher.SearchWithSameNames(args);
                foreach (var fileAndCount in files.OrderByDescending(f => f.Count))
                {
                    Console.WriteLine("{0}: {1}", fileAndCount.FileName, fileAndCount.Count);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Oops, error happened!");
                Console.WriteLine("Dedails: {0}", e.Message);
            }
        }

        private static bool ValidateArguments(string[] args)
        {
            bool ok = true;
            foreach (var path in args)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("Error: directory '{0}' does not exist", path);
                    ok = false;
                }
            }
            return ok;
        }

        private static void Usage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("> findfile [-size] path1 [path2 pathN]");
        }
    }
}

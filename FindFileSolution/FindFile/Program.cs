using System;
using System.Collections.Generic;
using System.IO;

namespace Eleks.Demo
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return 1;
            }

            if (!ValidateArguments(args))
            {
                return 1;
            }

            try
            {
                DoSearch(args);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine("Oops, error happened!");
                Console.Error.WriteLine("Dedails: {0}", e.Message);
            }

            return 0;
        }

        private static void DoSearch(IEnumerable<string> args)
        {
            var filesFinder = new FilesFinder();
            var filesWithSameName = filesFinder.SearchWithSameNames(args);
            foreach (var fileAndCount in filesWithSameName)
            {
                Console.WriteLine("{0}: {1}", fileAndCount.FileName, fileAndCount.Count);
            }
        }

        private static bool ValidateArguments(IEnumerable<string> args)
        {
            bool ok = true;
            foreach (var path in args)
            {
                if (!Directory.Exists(path))
                {
                    Console.Error.WriteLine("Error: directory '{0}' does not exist", path);
                    ok = false;
                }
            }
            return ok;
        }

        private static void Usage()
        {
            Console.WriteLine(@"
Use findfile to find files with the same name in different directories
Usage:
> findfile path1 [path2] [path3] [pathN]");
        }
    }
}

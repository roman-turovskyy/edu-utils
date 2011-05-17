using System;
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

            var parser = new ArgumentsParser(args);
            if (!parser.ParseArguments())
            {
                Console.Error.WriteLine(parser.ErrorMessage);
                return;
            }

            try
            {
                var searcher = new FileSearcher(parser);
                var files = searcher.SearchWithSameNames();
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

        private static void Usage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("> findfile [-size] path1 [path2 pathN]");
        }
    }
}

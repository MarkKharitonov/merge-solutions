using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolutionMerger.Parsers;
using SolutionMerger.Utils;

namespace SolutionMerger
{
    public static class Program
    {
        static int Main(string[] args)
        {
            /*
            args = new [] {"/config", "solutions.txt"};

            Console.WriteLine("#################");
            args.ToList().ForEach(Console.WriteLine);
            Console.WriteLine("#################");
            */
            
            var nonstop = false;
            var outputSlnPath = "merged.sln";
            var fixDupeGuids = false;
            var solutionNames = new List<string>();

            for(var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "/?":
                    case "/help":
                    case "/h":
                        break;
                    case "/nonstop":
                        nonstop = true;
                        break;
                    case "/out":
                        outputSlnPath = args[i + 1];
                        i++;
                        break;
                    case "/fix":
                        fixDupeGuids = true;
                        break;
                    case "/config":
                        solutionNames.AddRange(File.ReadAllLines(args[i + 1]).Where(fn => !string.IsNullOrEmpty(fn)));
                        i++;
                        break;
                    default:
                        solutionNames.AddRange(args.Skip(i));
                        i = args.Length;
                        break;
                }
            }

            if(solutionNames.Count == 0)
            {
                OutputHelp();
                return -1;
            }

            if(fixDupeGuids)
            {
                Console.WriteLine("Program is going to modify lots of various .sln and .proj files");
                Console.WriteLine("Please make sure that you have a backup copy");
                Console.WriteLine("Press ENTER to continue or anything else to stop ...");
                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Enter)
                    return -1;
            }

            string warnings;
            string errors = "";

            if(fixDupeGuids)
                ProjectReferenceFixer.FixAllSolutions(solutionNames.Select(SolutionInfo.Parse).ToArray(), out errors);

            outputSlnPath = Path.GetFullPath(outputSlnPath);
            var aggregateSolution = SolutionInfo.MergeSolutions(Path.GetFileNameWithoutExtension(outputSlnPath), Path.GetDirectoryName(outputSlnPath), out warnings, solutionNames.Select(SolutionInfo.Parse).ToArray());
            aggregateSolution.Save();

            Console.WriteLine("Merged solution: {0}", outputSlnPath);

            if (!string.IsNullOrWhiteSpace(errors))
            {
                Console.WriteLine("ERRORS found:");
                Console.Write(errors);
                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
                return -3;
            }

            if (!string.IsNullOrWhiteSpace(warnings))
            {
                Console.WriteLine("WARNINGS found:");
                Console.Write(warnings);
                Console.WriteLine("You might want to try running SolutionMerger with /fix parameter. Execute SolutionMerger.exe /help for more details.");
                if (!nonstop)
                {
                    Console.WriteLine("Press a key to exit...");
                    Console.ReadKey();
                }
                return -2;
            }

            return 0;
        }

        private static void OutputHelp()
        {
            Console.WriteLine("Howto use this:");
            Console.WriteLine("merge-solutions.exe [/nonstop] [/fix] [/config solutionlist.txt] [/out merged.sln] [solution1.sln solution2.sln ...]");
            Console.WriteLine("        /fix: Regenerates duplicate project guids and replaces them in corresponding project/solution files");
            Console.WriteLine("              requires write-access to project and solution files");
            Console.WriteLine("        /config solutionlist.txt: Takes list of new-line separated solution paths from solutionlist.txt file");
            Console.WriteLine("        /out merged.sln: path to output solution file. Default is 'merged.sln'");
            Console.WriteLine("        /nonstop: don't prompt for keypress if there were errors/warnings");
            Console.WriteLine("        solution?.sln - list of solutions to be merged");
        }
    }
}

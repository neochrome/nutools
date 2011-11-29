using System;
using System.Reflection;
using NuTools.Common;

[assembly: AssemblyTitle("du")]
[assembly: AssemblyDescription("a estimate file space usage utility")]

namespace NuTools.Du
{
    class Du
    {
        public static int Main(string[]args)
        {
            var opts = new OptionParser();
            opts.Header = "Summarize disk usage of each FILE, recursively for directories.";

            opts.Args<string>("FILE", "").Do(files => { });

            opts.On("help", "display this help text and exit").Do(() =>
            {
                opts.WriteUsage(Console.Out);
                Environment.Exit(0);
            });
            opts.On("version", "print version information and exit").Do(() =>
			{
				opts.WriteVersionInfo(Console.Out);
				Environment.Exit(0);
			});

            opts.Footer = "Display values are in units of the first available SIZE from --block-size,\n";
            opts.Footer += "and the DU_BLOCK_SIZE, BLOCK_SIZE and BLOCKSIZE environment variables.\n";
            opts.Footer += "Otherwise, units default to 1024 bytes (or 512 if POSIXLY_CORRECT is set).\n";
            opts.Footer += "\n";
            opts.Footer += "SIZE may be (or may be an integer optionally followed by) one of following:\n";
            opts.Footer += "KB 1000, K 1024, MB 1000*1000, M 1024*1024, and so on for G, T, P, E, Z, Y.\n";

            if (!opts.Parse(args))
            {
                Console.WriteLine("One or more arguments are invalid.");
                opts.WriteUsage(Console.Error);
                Environment.Exit(1);
            }

            // Code goes here ^_^

            return 0;
        }
    }
}

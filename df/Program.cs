using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using NuTools.Common;

[assembly: AssemblyTitle("df")]
[assembly: AssemblyDescription("a disk space utility")]

namespace NuTools.Df
{
	class Program
	{
	    public static int Main(string[] args)
		{
		    var humanReadable = false;
		    var printFileSystemType = false;
            var limitToType = "";
    	    var excludeType = "";

			var opts = new OptionParser();
			opts.Header = "Show information about the file system on which each FILE resides,\nor all file systems by deafult.";

            opts.Args<string>("FILE", "").Do(files => { });

			opts.On("human-readable", 'h', "print sizes in human readable format (e.g., 1K 234M 2G)").Do(() => humanReadable = true);
            opts.On("type", 't', "limit listing to file systems of type TYPE").WithArg<string>("TYPE").Do(arg => limitToType = arg);
            opts.On("print-type", 'T', "print file system type").Do(() => printFileSystemType = true);
            opts.On("exclude-type", 'x', "limit listing to file systems not of type TYPE").WithArg<string>("TYPE").Do(arg => excludeType = arg);

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
		    opts.Footer += "and the DF_BLOCK_SIZE, BLOCK_SIZE and BLOCKSIZE environment variables.\n";
		    opts.Footer += "Otherwise, units default to 1024 bytes (or 512 if POSIXLY_CORRECT is set).\n";
		    opts.Footer += "\n";
		    opts.Footer += "SIZE may be (or may be an integer optionally followed by) one of following:\n";
		    opts.Footer += "KB 1000, K 1024, MB 1000*1000, M 1024*1024, and so on for G, T, P, E, Z, Y.\n";
			
            if (!opts.Parse(args))
            {
                Console.WriteLine("One or more arguments are invalid.");
                opts.WriteUsage(Console.Out);
                Environment.Exit(1);
            }

		    var drivesToEnumerate = GetDrives(limitToType, excludeType);
		    var driveSummary = new DriveSummary(drivesToEnumerate, humanReadable, printFileSystemType);
            Console.Write(driveSummary.Render());

			return 0;
		}

        private static IEnumerable<IDrive> GetDrives(string limitToType, string excludeType)
        {
            var drives = new List<IDrive>();

            foreach (var info in DriveInfo.GetDrives())
            {
                var drive = Drive.LoadFrom(info);

                if (drive is NotSupportedDrive)
                    continue;
                if (!string.IsNullOrEmpty(limitToType) && drive.Format.ToLower() != limitToType.ToLower())
                    continue;
                if (!string.IsNullOrEmpty(excludeType) && drive.Format.ToLower() == excludeType.ToLower())
                    continue;

                drives.Add(Drive.LoadFrom(info));
            }

            return drives;
        }
	}
}
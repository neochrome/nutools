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

			var opts = new OptionParser();
			opts.Header = "Show information about the file system on which each FILE resides,\nor all file systems by deafult.";
			opts.On("help", "display this help text and exit").Do(() => {
				opts.WriteUsage(Console.Out);
				Environment.Exit(0);
			});
			opts.On("human-readable", 'h', "print sizes in human readable format (e.g., 1K 234M 2G)").Do(() => humanReadable = true);
		    opts.On("print-type", 'T', "print file system type").Do(() => printFileSystemType = true);
            opts.On("type", 't', "limit listing to file systems of type TYPE").WithArg<string>("TYPE").Do(arg => limitToType = arg);
            opts.On("version", 'V', "print version information and exit").Do(() =>
			{
				opts.WriteVersionInfo(Console.Out);
				Environment.Exit(0);
			});
			
            if (!opts.Parse(args))
            {
                Console.WriteLine("One or more arguments are invalid.");
                opts.WriteUsage(Console.Out);
                Environment.Exit(1);
            }

		    var drivesToEnumerate = GetDrives(limitToType);
		    var driveSummary = new DriveSummary(drivesToEnumerate, humanReadable, printFileSystemType);
            Console.Write(driveSummary.Render());

			return 0;
		}

        private static IEnumerable<IDrive> GetDrives(string limitToType)
        {
            var drives = new List<IDrive>();

            foreach (var info in DriveInfo.GetDrives())
            {
                var drive = Drive.LoadFrom(info);

                if (drive is NotSupportedDrive)
                    continue;
                if (!string.IsNullOrEmpty(limitToType) && drive.Format.ToLower() != limitToType.ToLower())
                    continue;

                drives.Add(Drive.LoadFrom(info));
            }

            return drives;
        }
	}
}
using System;
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

			var opts = new OptionParser();
			opts.Banner = "Usage: df [OPTION]... [FILE]...";
			opts.Banner += "\nShow information about the file system on which each FILE resides,\nor all file systems by deafult.";
			opts.On("help", "display this help text and exit").Do(() => { PrintUsageAndExit(opts.Summary()); });
			opts.On("human-readable", 'h', "print sizes in human readable format (e.g., 1K 234M 2G)").Do(() => humanReadable = true);
			opts.Parse(args);

			var driveRepository = new DriveRepository();
			var driveSummary = CreateDriveSummary(driveRepository, humanReadable);

			Console.Write(driveSummary.Render());

			return 0;
		}

		private static DriveSummary CreateDriveSummary(DriveRepository driveRepository, bool humanReadable)
		{
			return humanReadable
				? new DriveSummary(driveRepository.GetDrives(), new FileSizeFormatProvider())
				: new DriveSummary(driveRepository.GetDrives());
		}

		private static void PrintUsageAndExit(string summary)
		{
			Console.WriteLine(summary);
			Environment.Exit(0);
		}
	}
}
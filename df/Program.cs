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
			opts.On("help", "display this help text and exit").Do(() => PrintUsageAndExit());
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

		private static void PrintUsageAndExit()
		{
			Console.WriteLine("Usage: df [OPTION]... [FILE]...");
			Console.WriteLine("Show information about the file system on which each FILE resides, or all file systems by deafult.");
			Environment.Exit(0);
		}
	}
}
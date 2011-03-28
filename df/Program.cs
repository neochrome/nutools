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
			var opts = new OptionParser();
			opts.On("help", "display this help text and exit").Do(() => PrintUsageAndExit());
			opts.Parse(args);

			var driveRepository = new DriveRepository();
			var driveSummary = new DriveSummary(driveRepository.GetDrives());
			Console.Write(driveSummary.Render());

			return 0;
		}

		private static void PrintUsageAndExit()
		{
			Console.WriteLine("Usage: df [OPTION]... [FILE]...");
			Console.WriteLine("Show information about the file system on which each FILE resides, or all file systems by deafult.");
			Environment.Exit(0);
		}
	}
}
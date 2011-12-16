using System;
using System.Collections.Generic;
using System.IO;

using NuTools.Common;

namespace NuTools.Df
{
	public class Df : ICommand
	{
		public void WithOptions(OptionParser opts)
		{
			opts.Header = "Show information about the file system on which each FILE resides,\nor all file systems by deafult.";

			opts.Args<string>("FILE", "").Do(files => { });

			opts.On("block-size", 'B', "use SIZE-byte blocks").WithArg<int>("SIZE").Do(arg => { customBlockSize = true; blockSize = arg; });
			opts.On("human-readable", 'h', "print sizes in human readable format (e.g., 1K 234M 2G)").Do(() => humanReadable = true);
			opts.On("si", 'H', "likewise, but use powers of 1000 not 1024").Do(() => humanReadableWithSi = true);
			opts.On("portability", 'P', "use the POSIX output format").Do(() => posixFormat = true);
			opts.On("type", 't', "limit listing to file systems of type TYPE").WithArg<string>("TYPE").Do(arg => limitToType = arg);
			opts.On("print-type", 'T', "print file system type").Do(() => printFileSystemType = true);
			opts.On("exclude-type", 'x', "limit listing to file systems not of type TYPE").WithArg<string>("TYPE").Do(arg => excludeType = arg);

			opts.Footer = "Display values are in units of the first available SIZE from --block-size,\n";
			opts.Footer += "and the DF_BLOCK_SIZE, BLOCK_SIZE and BLOCKSIZE environment variables.\n";
			opts.Footer += "Otherwise, units default to 1024 bytes (or 512 if POSIXLY_CORRECT is set).\n";
			opts.Footer += "\n";
			opts.Footer += "SIZE may be (or may be an integer optionally followed by) one of following:\n";
			opts.Footer += "KB 1000, K 1024, MB 1000*1000, M 1024*1024, and so on for G, T, P, E, Z, Y.\n";
		}
		
		public int Execute()
		{
			var drivesToEnumerate = GetDrives(limitToType, excludeType);
			var driveSummary = new DriveSummary(
				drivesToEnumerate,
				humanReadable,
				humanReadableWithSi,
				posixFormat,
				printFileSystemType,
				customBlockSize,
				blockSize
			);
			Console.Write(driveSummary.Render());

			return 0;
		}

		bool humanReadable = false;
		bool humanReadableWithSi = false;
		bool printFileSystemType = false;
		string limitToType = "";
		string excludeType = "";
		bool posixFormat = false;
		int blockSize = 1024;
		bool customBlockSize = false;

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
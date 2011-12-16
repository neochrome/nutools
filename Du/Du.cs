using NuTools.Common;

namespace NuTools.Du
{
	public class Du : ICommand
	{
		public void WithOptions(OptionParser opts)
		{
			opts.Header = "Summarize disk usage of each FILE, recursively for directories.";

			opts.Args<string>("FILE", "").Do(files => { });

			opts.Footer = "Display values are in units of the first available SIZE from --block-size,\n";
			opts.Footer += "and the DU_BLOCK_SIZE, BLOCK_SIZE and BLOCKSIZE environment variables.\n";
			opts.Footer += "Otherwise, units default to 1024 bytes (or 512 if POSIXLY_CORRECT is set).\n";
			opts.Footer += "\n";
			opts.Footer += "SIZE may be (or may be an integer optionally followed by) one of following:\n";
			opts.Footer += "KB 1000, K 1024, MB 1000*1000, M 1024*1024, and so on for G, T, P, E, Z, Y.\n";
		}

		public int Execute()
		{
			// Code goes here ^_^
			return 0;
		}
	}
}

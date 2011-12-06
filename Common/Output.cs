using System;
using System.IO;

namespace NuTools.Common
{
	public class Output
	{
		public Output(string filename, FileMode mode)
		{
			StandardOutput = IsStandardOutput(filename);
			Name = StandardOutput ? "(standard output)" : filename;
			Mode = mode;
			Open = () => StandardOutput ? Console.OpenStandardOutput() : File.Open(Name, Mode);
		}
		
		public string Name { get; private set;}
		public FileMode Mode { get; private set; }
		public Func<Stream> Open { get; private set; }
		public bool StandardOutput { get; private set; }
		
		public static bool IsStandardOutput(string filename)
		{
			return filename == "-";
		}
		
		public static Output FromStandardOutput()
		{
			return new Output("-", FileMode.Append);
		}
	}
}

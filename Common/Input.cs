using System;
using System.IO;

namespace NuTools.Common
{
	public class Input
	{
		public Input(string filename)
		{
			StandardInput = IsStandardInput(filename);
			Name = StandardInput ? "(standard input)" : filename;
			Open = () => StandardInput ? Console.OpenStandardInput() : File.OpenRead(Name);
		}
		
		public string Name { get; private set;}
		public Func<Stream> Open { get; private set; }
		public bool StandardInput { get; private set; }
		
		public static bool IsStandardInput(string filename)
		{
			return filename == "-";
		}
		
		public static Input FromStandardInput()
		{
			return new Input("-");
		}
	}
}

using System;
using System.Reflection;
using NuTools.Common;

[assembly: AssemblyTitle("grep")]
[assembly: AssemblyDescription("a grep utility")]

namespace NuTools.Grep
{
	class Program
	{
		public static void Main(string[] args)
		{
            var opts = new OptionParser();

			Console.WriteLine("Hello World!");
		}
	}
}
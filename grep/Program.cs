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
			opts.Required.Arg<string>("PATTERN", "").Do(_ => { });
			opts.Arg<string>("FILE", "").Do(_ => { });
			opts.In("Regexp selection and interpretation", g => {
				g.On("extended-regexp", 'E', "PATTERN is an extended regular expression").Do(() => { });
				g.On("fixed-strings", 'F', "PATTERN is a set of newline-separated strings").Do(() => { });
				g.On("basic-regexp", 'G', "PATTERN is a basic regular expression").Do(() => { });
				g.On("regexp", 'e', "use {ARG} as a regular expression").WithArg<string>("PATTERN").Do(_ => { });
				g.On("file", 'f', "obtain PATTERN from {ARG}").WithArg<string>("FILE").Do(_ => { });
			});
			opts.In("Miscellaneous", g => {
				g.On("no-messages", 's', "suppress error messages").Do(() => { });
				g.On("invert-match", 'v', "select non-matching lines").Do(() => { });
				g.On("help", "display this help and exit").Do(() => {
					opts.WriteUsage(Console.Out);
					Environment.Exit(0);
				});
				g.On("version", 'V', "print version information and exit").Do(() => {
					opts.WriteVersionInfo(Console.Out);
					Environment.Exit(0);
				});
			});
			opts.Parse(args);
		}
	}
}
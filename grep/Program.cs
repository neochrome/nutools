using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NuTools.Common;
using System.IO;

[assembly: AssemblyTitle("grep")]
[assembly: AssemblyDescription("a grep utility")]

namespace NuTools.Grep
{
	class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var settings = new Settings();

				#region Option parsing
				var opts = new OptionParser();
				opts.Header = "Search for PATTERN in each FILE or standard input.\n";
				opts.Header += "Example: grep -i 'hello world' menu.h main.c";

				opts.Required.Arg<string>("PATTERN", "").Do(v => settings.Pattern = v);
				opts.Args<string>("FILE", "").Do(v => settings.Files = v);

				opts.In("Regexp selection and interpretation", g =>
				{
					g.On("ignore-case", 'i', "ignore case distinctions").Do(() => settings.RegexOptions |= RegexOptions.IgnoreCase);
				});
				opts.In("Miscellaneous", g =>
				{
					g.On("no-messages", 's', "n/a - suppress error messages").Do(() => { });
					g.On("invert-match", 'v', "n/a - select non-matching lines").Do(() => { });
					g.On("help", "display this help and exit").Do(() =>
					{
						opts.WriteUsage(Console.Out);
						Environment.Exit(0);
					});
					g.On("version", 'V', "print version information and exit").Do(() =>
					{
						opts.WriteVersionInfo(Console.Out);
						Environment.Exit(0);
					});
				});

				opts.Footer = "With no FILE, or when FILE is -, read standard input.  If less than\n";
				opts.Footer += "two FILEs given, assume -h.  Exit status is 0 if match, 1 if no match,\n";
				opts.Footer += "and 2 if trouble.";

				if (!opts.Parse(args))
				{
					opts.WriteUsage(Console.Out);
					Environment.Exit(2);
				}
				#endregion

				var regex = new Regex(settings.Pattern, settings.RegexOptions);
				var fileNames = new Glob(Environment.CurrentDirectory);
				settings.Files.Each(fileNames.Include);
				
				var anyMatch = false;
				fileNames.Each(fileName => {
					using (var file = File.OpenText(fileName))
					{
						string line;
						var lineNumber = 0;
						while ((line = file.ReadLine()) != null)
						{
							lineNumber++;
							if (regex.IsMatch(line))
							{
								anyMatch = true;
								Console.Out.WriteLine("[{0}:{1}] {2}", fileName, lineNumber, line);
							}
						}
					}
				});

				Environment.Exit(anyMatch ? 0 : 1);
			}
			catch (Exception ex)
			{
				Console.Out.WriteLine(ex.Message);
				Environment.Exit(2);
			}
		}
	}

	struct Settings
	{
		public RegexOptions RegexOptions;
		public string Pattern;
		public string[] Files;
	}
}
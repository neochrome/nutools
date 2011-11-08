using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using NuTools.Common;

[assembly: AssemblyTitle("grep")]
[assembly: AssemblyDescription("a grep utility")]

namespace NuTools.Grep
{
	class Program
	{
		public static void Main(string[] args)
		{
			var settings = new Settings();
			try
			{
				#region Option parsing
				var opts = new OptionParser();
				opts.Header = "Search for PATTERN in each FILE or standard input.\n";
				opts.Header += "Example: grep -i 'hello world' menu.h main.c";

				opts.Required.Arg<string>("PATTERN", "").Do(pattern => settings.Pattern = pattern);
				opts.Args<string>("FILE", "").Do(settings.Files.AddRange);

				opts.In("Regexp selection and interpretation", g =>
				{
					g.On("ignore-case", 'i', "ignore case distinctions").Do(() => settings.RegexOptions |= RegexOptions.IgnoreCase);
				});
				opts.In("Miscellaneous", g =>
				{
					g.On("no-messages", 's', "suppress error messages").Do(() => settings.SuppressErrorMessages = true);
					g.On("invert-match", 'v', "select non-matching lines").Do(() => settings.InvertMatch = true);
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
				opts.In("Output control", g => {
					g.On("line-number", 'n', "print line number with output lines").Do(() => settings.Output.LineNumbers = true);
					g.On("with-filename", 'H', "print the filename for each match").Do(() => settings.Output.FileNames = true);
					g.On("no-filename", 'h', "print line number with output lines").Do(() => settings.Output.FileNames = false);
					g.On("files-without-match", 'L', "only print FILE names containing no match").Do(() => settings.Output.OnlyFileNames = OnlyFileNames.NonMatching);
					g.On("files-with-matchese", 'l', "only print FILE names containing matches").Do(() => settings.Output.OnlyFileNames = OnlyFileNames.Matching);
				});

				opts.Footer = "With no FILE, or when FILE is -, read standard input.  If less than\n";
				opts.Footer += "two FILEs given, assume -h. Exit status is 0 if match, 1 if no match,\n";
				opts.Footer += "and 2 if trouble.";

				if (!opts.Parse(args))
				{
					if (!settings.SuppressErrorMessages)
						opts.WriteUsage(Console.Error);
					Environment.Exit(2);
				}
				#endregion

				#region Grepping
				var regex = new Regex(settings.Pattern, settings.RegexOptions);

				var inputs = new Inputs(settings.Files);
				if(!settings.Output.FileNames.HasValue)
					settings.Output.FileNames = inputs.Any;

				var anyMatch = false;
				inputs.Process((name, file) => {
					var fileHasNoMatch = true;
					string line;
					var lineNumber = 0;
					while ((line = file.ReadLine()) != null)
					{
						lineNumber++;
						if (regex.IsMatch(line) != settings.InvertMatch)
						{
							fileHasNoMatch = false;
							anyMatch = true;
							if(settings.Output.OnlyFileNames == OnlyFileNames.No)
							{
								if (settings.Output.FileNames.Value)
									Console.Out.Write("{0}:", name);
								if (settings.Output.LineNumbers)
									Console.Out.Write("{0}:", lineNumber);
								Console.Out.WriteLine(line);
							}
							else if(settings.Output.OnlyFileNames == OnlyFileNames.Matching){
								Console.WriteLine(name);
								break;
							}
						}
					}
					if (settings.Output.OnlyFileNames == OnlyFileNames.NonMatching && fileHasNoMatch)
					{
						Console.WriteLine(name);
					}
					return true;
				});
				#endregion

				Environment.Exit(anyMatch ? 0 : 1);
			}
			catch (Exception ex)
			{
				if(!settings.SuppressErrorMessages)
					Console.Error.WriteLine(ex.Message);
				Environment.Exit(2);
			}
		}
	}

	enum OnlyFileNames
	{
		No = 0,
		Matching,
		NonMatching
	}

	class Settings
	{
		public RegexOptions RegexOptions;
		public string Pattern;
		public List<string> Files = new List<string>();
		public bool InvertMatch;
		public bool SuppressErrorMessages;
		
		public OutputSettings Output;
		public struct OutputSettings
		{
			public bool LineNumbers;
			public bool? FileNames;
			public OnlyFileNames OnlyFileNames;
		}
	}
}

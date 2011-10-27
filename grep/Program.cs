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
			var settings = new Settings();
			try
			{
				#region Option parsing
				var opts = new OptionParser();
				opts.Header = "Search for PATTERN in each FILE or standard input.\n";
				opts.Header += "Example: grep -i 'hello world' menu.h main.c";

				opts.Required.Arg<string>("PATTERN", "").Do(pattern => settings.Pattern = pattern);
				opts.Args<string>("FILE", "").Do(files => settings.Files = files);

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
					g.On("with-filename", 'H', "print the filename for each match").Do(() => settings.Output.Filenames = true);
					g.On("no-filename", 'h', "print line number with output lines").Do(() => settings.Output.Filenames = false);
				});

				opts.Footer = "With no FILE, or when FILE is -, read standard input.  If less than\n";
				opts.Footer += "two FILEs given, assume -h.  Exit status is 0 if match, 1 if no match,\n";
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
				var fileNames = new Glob(Environment.CurrentDirectory);
				settings.Files.Each(fileNames.Include);
				if(!settings.Output.Filenames.HasValue)
					settings.Output.Filenames = fileNames.Count > 1;
		
				var anyMatch = false;
				fileNames.Each(fileName => {
					using (var file = File.OpenText(fileName))
					{
						string line;
						var lineNumber = 0;
						while ((line = file.ReadLine()) != null)
						{
							lineNumber++;
							if (regex.IsMatch(line) != settings.InvertMatch)
							{
								anyMatch = true;
								if (settings.Output.Filenames.Value)
									Console.Out.Write("{0}:", fileName);
								if (settings.Output.LineNumbers)
									Console.Out.Write("{0}:", lineNumber);
								Console.Out.WriteLine(line);
							}
						}
					}
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

	struct Settings
	{
		public RegexOptions RegexOptions;
		public string Pattern;
		public string[] Files;
		public bool InvertMatch;
		public bool SuppressErrorMessages;
		
		public OutputSettings Output;
		public struct OutputSettings
		{
			public bool LineNumbers;
			public bool? Filenames;
		}
	}
}
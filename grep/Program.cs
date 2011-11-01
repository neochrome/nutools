using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
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
			Func<string, bool> fromStdIn = f => f == "-";
			var settings = new Settings();
			try
			{
				#region Option parsing
				var opts = new OptionParser();
				opts.Header = "Search for PATTERN in each FILE or standard input.\n";
				opts.Header += "Example: grep -i 'hello world' menu.h main.c";

				opts.Required.Arg<string>("PATTERN", "").Do(pattern => settings.Pattern = pattern);
				opts.Args<string>("FILE", "").Do(files => {
					if(files.Count(fromStdIn) > 1)
						throw new ArgumentException("Only one read from stdin (-) allowed.", "FILE");
					settings.Files.AddRange(files);
				});

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
				var fileNames = new Glob(Environment.CurrentDirectory);

				settings.Files.Except(settings.Files.Where(fromStdIn)).Each(fileNames.Include);
				var inputs = fileNames.Select(f => new IOItem{ Name = f, Open = () => File.OpenText(f) }).ToList();
				if(settings.Files.Any(fromStdIn))
					inputs.Insert(0, new IOItem { Name = "(standard input)", Open = () => new StreamReader(Console.OpenStandardInput()) });

				if(!settings.Output.Filenames.HasValue)
					settings.Output.Filenames = inputs.Count > 1;

				var anyMatch = false;
				inputs.Each(input => {
					using (var file = input.Open())
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
									Console.Out.Write("{0}:", input.Name);
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
	
	class IOItem
	{
		public string Name;
		public Func<StreamReader> Open;
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
			public bool? Filenames;
		}
	}
}

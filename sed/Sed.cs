using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using NuTools.Common;

namespace NuTools.Sed
{
	public class Sed : ICommand
	{
		public void Main(string[] args)
		{
			var settings = new Settings();
			try
			{
				#region Option parsing
				var opts = new OptionParser();
				opts.Header = "Search for PATTERN and replace with REPLACE in each FILE or standard input.\n";
				opts.Header += "Example: sed 'hello world' 'goodbye world' menu.h main.c";

				opts.Required.Arg<string>("PATTERN", "{0} is a .NET/Mono Regular Expression string.").Do(pattern => settings.Pattern = pattern);
				opts.Required.Arg<string>("REPLACE", "").Do(replace => settings.Replace = replace);
				opts.Args<string>("FILE", "").Do(settings.Files.AddRange);

				opts.In("Regexp selection and interpretation", g =>
				{
					g.On("ignore-case", 'i', "ignore case distinctions").Do(() => settings.RegexOptions |= RegexOptions.IgnoreCase);
				});
				opts.In("Miscellaneous", g =>
				{
					g.On("no-messages", 's', "suppress error messages").Do(() => settings.SuppressErrorMessages = true);
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

				opts.Footer = "With no FILE, or when FILE is -, read standard input.\n";
				opts.Footer += "Exit status is 0 on success and 2 if trouble.";

				opts.Parse(args);
				#endregion
				
				#region Sedding
				
				var regex = new Regex(settings.Pattern, settings.RegexOptions);
				var inputs = new Inputs(settings.Files);
				inputs.Process((_,file) => {
					string line;
					while ((line = file.ReadLine()) != null)
					{
						Console.Out.WriteLine(regex.Replace(line, settings.Replace));
					}
				});
				
				#endregion
				
				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				if(!settings.SuppressErrorMessages)
					Console.Error.WriteLine(ex.Message);
				Environment.Exit(2);
			}
		}
	}

	class Settings
	{
		public RegexOptions RegexOptions;
		public string Pattern;
		public string Replace;
		public List<string> Files = new List<string>();
		public bool SuppressErrorMessages;
	}
}

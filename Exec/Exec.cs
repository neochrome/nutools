using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using NuTools.Common;

namespace NuTools.Exec
{
	public class Exec : ICommand
	{
		public void WithOptions(OptionParser opts)
		{
			opts.Header = "Executes a COMMAND once for each line of input on STDIN.";
			opts.Required.Arg<string>("COMMAND", "{0} to execute.").Do(command => settings.Command = command);
			opts.Args<string>("ARGUMENTS", "Additional {0} to COMMAND. Use - to grab a line from STDIN.").Do(settings.Arguments.AddRange);
			opts.On("verbose", 'v', "Echo each execution of COMMAND on STDERR.").Do(() => settings.Verbose = true);
		}
		
		public int Execute()
		{
			try
			{
				var commandInfo = new ProcessStartInfo
				{
					CreateNoWindow = true,
					UseShellExecute = true,
					FileName = settings.Command,
					Arguments = "",
				};
				
				string line;
				while((line = Console.In.ReadLine()) != null)
				{
					var arguments = settings.Arguments
						.Select(arg => "\"" + (arg == "-" ? line : arg) + "\"")
						.Aggregate("", (acc, arg) => acc += " " + arg);

					if(settings.Verbose)
						Console.Error.WriteLine("{0} {1}", settings.Command, arguments);
					
					commandInfo.Arguments = arguments;
					var p = Process.Start(commandInfo);
					p.WaitForExit();
				}

				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
		}

		readonly Settings settings = new Settings();
	}

	class Settings
	{
		public string Command;
		public List<string> Arguments = new List<string>();
		public bool Verbose;
	}
}

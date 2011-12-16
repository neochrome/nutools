using System;
using System.Linq;
using NuTools.Common;

namespace NuTools
{
	class Bootstrap
	{
		static int Main(string[] args)
		{
			var opts = new OptionParser();
			opts.On("help", "display this help and exit").Do(() =>
			{
				opts.WriteUsage(Console.Out);
				Environment.Exit(0);
			});
			opts.On("version", 'V', "print version information and exit").Do(() =>
			{
				opts.WriteVersionInfo(Console.Out);
				Environment.Exit(0);
			});

			try
			{
				var commandType = App.Commands
				   .SingleOrDefault(NamedAsExecutable)
				   ?? typeof(UnknownCommand);

				var command = ((ICommand)Activator.CreateInstance(commandType));

				command.WithOptions(opts);
				opts.Parse(args);
				return command.Execute();
			}
			catch (OptionParserException ex)
			{
				Console.Error.WriteLine(ex.Message);
				opts.WriteUsageHeader(Console.Error);
				Console.Error.WriteLine("\nTry `{0} --help' for more options.", App.CommandName);
				return 2;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
		}

		static bool NamedAsExecutable(Type commandType)
		{
			return string.Equals(commandType.Name, App.CommandName, StringComparison.InvariantCultureIgnoreCase);
		}

	}
}
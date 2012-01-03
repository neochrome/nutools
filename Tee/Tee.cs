using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuTools.Common;

namespace NuTools.Tee
{
	public class Tee : ICommand
	{
		public void WithOptions(OptionParser opts)
		{
			opts.Header = "Copy standard input to each FILE, and also to standard output.";

			opts.Args<string>("FILE", "").Do(settings.Files.AddRange);

			opts.On("append", 'a', "Append to the given FILEs, do not overwrite").Do(() => settings.FileMode = FileMode.Append);

			opts.Footer = "With no FILE, or when a FILE is -, copy again to standard output.";
		}

		public int Execute()
		{
			var outputs = new List<StreamWriter>();
			try
			{
				settings
					.Files
					.Select(f => new Output(f, settings.FileMode).Open())
					.Select(o => new StreamWriter(o))
					.Each(outputs.Add);
				string line;
				while ((line = Console.In.ReadLine()) != null)
				{
					Console.Out.WriteLine(line);
					outputs.Each(o => o.WriteLine(line));
				}
				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				return 1;
			}
			finally
			{
				outputs.Each(o => o.Dispose());
			}
		}

		readonly Settings settings = new Settings();
	}

	class Settings
	{
		public List<string> Files = new List<string>();
		public FileMode FileMode = FileMode.Create;
	}
}

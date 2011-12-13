using System;
using System.IO;
using System.Linq;

namespace NuTools
{
	class Bootstrap
	{
		static void Main(string[] args)
		{
			var command = All.Commands
			   .SingleOrDefault(NamedAsExecutable)
			   ?? typeof(UnknownCommand);

			((ICommand)Activator.CreateInstance(command)).Main(args);
		}

		static bool NamedAsExecutable(Type commandType)
		{
			return string.Equals(commandType.Name, Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName), StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
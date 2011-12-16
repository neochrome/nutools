using System;
using NuTools.Common;

namespace NuTools
{
	class UnknownCommand : ICommand
	{
		public void WithOptions(OptionParser opts) { }

		public int Execute()
		{
			Console.WriteLine("Unknown command: {0}", App.CommandName);
			return 1;
		}
	}
}
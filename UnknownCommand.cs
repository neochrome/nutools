using System;

namespace NuTools
{
	class UnknownCommand : ICommand
	{
		public void Main(string[] args)
		{
			Console.WriteLine("Unknown command: {0}", AppDomain.CurrentDomain.FriendlyName);
			Environment.Exit(1);
		}
	}
}
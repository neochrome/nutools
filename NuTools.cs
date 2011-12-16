using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using NuTools.Common;

namespace NuTools
{
	public class NuTools : ICommand
	{
		public void WithOptions(OptionParser opts)
		{
			opts.Required.Arg<Command>("COMMAND", "creates symlinks for all contained commands").Do(c => command = c);
			opts.On("force", 'f', "forces creation/removal of symlinks").Do(() => force = true);
		}

		public int Execute()
		{
			var action = new Dictionary<Command, Action<string>> {
				{ Command.Install, CreateSymLink },
				{ Command.Uninstall, RemoveSymLink },
			}[command];

			App.Commands
				.Except<NuTools>()
				.Select(c => c.Name.ToLower() + ".exe")
				.Each(action);
			return 0;
		}

		private bool force;
		private Command command;
		enum Command { Install, Uninstall }

		private void CreateSymLink(string command)
		{
			var source = AppDomain.CurrentDomain.FriendlyName;
			Console.Write("Linking {0} ==> {1} ", source, command);
			if (File.Exists(command) && !force)
			{
				Console.WriteLine("<== already exists, please remove and try again (or use --force)");
			}
			else
			{
				Console.WriteLine(CreateSymbolicLink(command, source, SymbolicLinkType.File) ? "- success" : "- failed");
			}
		}

		private void RemoveSymLink(string command)
		{
			if(!File.Exists(command)) return;
			Console.Write("Removing {0} ", command);
			var symlink = (File.GetAttributes(command) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
			if(symlink || force)
			{
				File.Delete(command);
				Console.WriteLine("success");
			}
			else
			{
				Console.WriteLine("<== not a symlink, use --force to remove");
			}
		}

		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink(string linkFileName, string targetFileName, SymbolicLinkType flags);
		enum SymbolicLinkType : int { File = 0, Directory = 1 }
	}
}

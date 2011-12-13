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
		public void Main(string[] args)
		{
			var opts = new OptionParser();
			opts.Required.Arg<Command>("COMMAND", "creates symlinks for all contained commands").Do(command => this.command = command); 
			opts.On("force", 'f', "forces creation/removal of symlinks").Do(() => force = true);
			
			try
			{
				opts.Parse(args);

				var action = new Dictionary<Command, Action<string>> {
					{ Command.Install, CreateSymLink },
					{ Command.Uninstall, RemoveSymLink },
				}[command];

				All.Commands
					.Select(c => c.Name.ToLower() + ".exe")
					.Each(action);

				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				Environment.Exit(1);
			}
		}

		private bool force;
		private Command command;
		enum Command { Install, Uninstall }

		private void CreateSymLink(string command)
		{
			var source = AppDomain.CurrentDomain.FriendlyName;
			Console.Write("Linking {0} ==> {1} ", source, command);
			if (File.Exists(command))
			{
				Console.WriteLine("<== already exists, please remove and try again");
			}
			else
			{
				Console.WriteLine(CreateSymbolicLink(command, source, SymbolicLinkType.File) ? "- success" : "- failed");
			}
		}

		private void RemoveSymLink(string command)
		{
			Console.Write("Removing {0} ", command);
			var symlink = File.GetAttributes(command) != FileAttributes.Normal;
			if (symlink || force)
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

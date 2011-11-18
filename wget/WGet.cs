using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using NuTools.Common;

[assembly: AssemblyTitle("wget")]
[assembly: AssemblyDescription("a non-interactive network retriever")]

namespace NuTools.WGet
{
	class WGet
	{
		public static void Main(string[] args)
		{
			var settings = new Settings();
			try
			{
				#region Option parsing
				var opts = new OptionParser();

				opts.Required.Arg<string>("URL", "").Do(fetchUrl => {});

				opts.In("Startup", g =>
				{
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
				opts.In("Logging and input file", g =>
				{
					g.On("output-file", 'o', "log messages to {0}").WithArg<string>("FILE").Do(filename => {});
					g.On("apped-output", 'a', "append messages to {0}").WithArg<string>("FILE").Do(filename => {});
					g.On("quiet", 'q', "quiet (no output)").Do(() => {});
					g.On("debug", 'd', "print lots of debugging information").Do(() => {});
				});
			
				opts.In("Download", g =>
				{
					g.On("tries", 't', "set number of retries to {0} (0 - unlimited)").WithArg<int>("NUMBER").Do(retries => {});
					g.On("output-document", 'O', "write documents to {0}").WithArg<string>("FILE").Do(filename => {});
					g.On("no-clobber", 'n', "skip downloads that would download to existing files").Do(() => {});
					g.On("continue", 'c', "resume getting a partially-downloaded file").Do(() => {});
					g.On("timestamping", 'N', "don't re-retrieve files unless newer than local").Do(() => {});
					g.On("server-response", 'S', "print server response").Do(() => {});
					g.On("spider", "don't downloadanything").Do(() => {});
					g.On("timeout", 'T', "set all timeout values to {0}").WithArg<int>("SECONDS").Do(timeout => {});
					g.On("user", "set both ftp and http user to {0}").WithArg<string>("USER").Do(user => {});
					g.On("password", "set both ftp and http password to {0}").WithArg<string>("PASSWORD").Do(password => {});
					g.On("ask-password", "prompt for passwords").Do(() => {});
				});

				opts.In("HTTP options", g =>
				{
					g.On("http-user", "set http user to {0}").WithArg<string>("USER").Do(user => {});
					g.On("http-password", "set http password to {0}").WithArg<string>("PASSWORD").Do(password => {});
					g.On("ask-password", "prompt for passwords").Do(() => {});
					g.On("referer", "include `Referer: {0}' header in HTTP request").WithArg<string>("URL").Do(refererUrl => {});
					g.On("header", "insert {0} among the headers sent").WithArg<string>("STRING").Do(header => {});
					g.On("user-agent", 'U', "identify as {0} instead of Wget/VERSION").WithArg<string>("AGENT").Do(userAgent => {});
					g.On("post-data", "use the POST method; send {0} as the data").WithArg<string>("STRING").Do(postData => {});
					g.On("post-file", "use the POST method; send contents of {0}").WithArg<string>("FILE").Do(postDataFile => {});
				});
				
				if (!opts.Parse(args))
				{
					opts.WriteUsageHeader(Console.Error);
					Console.Error.WriteLine("\nTry `wget --help' for more options.");
					Environment.Exit(2);
				}
				#endregion

				#region Grepping
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

	enum OnlyFileNames
	{
		No = 0,
		Matching,
		NonMatching
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
			public bool? FileNames;
			public OnlyFileNames OnlyFileNames;
		}
	}
}

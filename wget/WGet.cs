using System;
using System.IO;
using System.Net;
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
			var opts = new OptionParser();
			try
			{
				#region Option parsing

				opts.Required.Arg<string>("URL", "").Do(settings.Urls.Add);

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
					g.On("output-file", 'o', "log messages to {0}").WithArg<string>("FILE").Do(filename => settings.Logging.WriteTo = filename);
					g.On("append-output", 'a', "append messages to {0}").WithArg<string>("FILE").Do(filename => settings.Logging.AppendTo = filename);
					g.On("quiet", 'q', "quiet (no output)").Do(() => { settings.Quiet = true;});
					g.On("debug", 'd', "print lots of debugging information").Do(() => settings.Logging.Debug = true);
				});
			
				opts.In("Download", g => {
					g.On("tries", 't', "set number of retries to {0} (0 - unlimited)").WithArg<int>("NUMBER").Do(retries => {});
					g.On("output-document", 'O', "write documents to {0}").WithArg<string>("FILE").Do(filename => settings.Download.OutputDocument = filename);
					g.On("no-clobber", 'n', "skip downloads that would download to existing files").Do(() => {});
					g.On("continue", 'c', "resume getting a partially-downloaded file").Do(() => {});
					g.On("progress", "select progress gauge {0}")
					 .WithArg<ProgressType>("TYPE")
					 .WithParseErrorMessage("--{0}: Invalid progress type '{1}'.\nValid choices are: None, Bar, Dot")
					 .Do(type => settings.Download.ProgressType = type);
					g.On("timestamping", 'N', "don't re-retrieve files unless newer than local").Do(() => {});
					g.On("server-response", 'S', "print server response").Do(() => {});
					g.On("spider", "don't download anything").Do(() => {});
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
				
				opts.Parse(args);
				#endregion

				#region Getting

				Action<string> log = text => { if(!settings.Quiet) Console.Out.WriteLine(text); };
				
				var wait = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
				
				using(var client = new WebClientWithInfo())
				{
					client.Headers[HttpRequestHeader.UserAgent] = settings.Http.UserAgent;
					client.Headers[HttpRequestHeader.Accept] = "*/*";
					
					client.GotResponse += (object _, WebClientWithInfo.ResponseEventArgs e) => {
						log("Got response...");
						log("Length: {0}".With(e.Response.ContentLength > -1 ? e.Response.ContentLength.ToString() : "unspecfied"));
						log("Saving to: '{0}'\n".With(settings.Download.OutputDocument));
					};
					
					client.DownloadProgressChanged += (object _, DownloadProgressChangedEventArgs e) => {
						log("progress: {0}/{1} ({2}%)".With(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage));
					};

					client.DownloadDataCompleted += (_, e) => wait.Set();

					settings.Urls.Each(url => {
						client.DownloadDataAsync(new Uri(url));
						// update progressbar etc
						wait.WaitOne();
					});
				};
				
				
				#endregion

				Environment.Exit(0);
			}
			catch (OptionParserException ex)
			{
				Console.Error.WriteLine(ex.Message);
				opts.WriteUsageHeader(Console.Error);
				Console.Error.WriteLine("\nTry `wget --help' for more options.");
				Environment.Exit(2);
			}
			catch (Exception ex)
			{
				if(!settings.Quiet)
					Console.Error.WriteLine(ex.Message);
				Environment.Exit(2);
			}
		}
	}
	
	public class WebClientWithInfo : WebClient
	{
		public class ResponseEventArgs : EventArgs
		{
			public WebResponse Response;
		}
		public event EventHandler<ResponseEventArgs> GotResponse;
		
		protected override WebResponse GetWebResponse (WebRequest request)
		{
			var response = base.GetWebResponse(request);
			OnGotResponse(new ResponseEventArgs { Response = response });
			return response;
		}
		
		protected virtual void OnGotResponse(ResponseEventArgs args)
		{
			if(GotResponse != null)
				GotResponse(this, args);
		}
	}
	
	enum ProgressType
	{
		None = 0,
		Bar,
		Dot
	}
	
	class Settings
	{
		public List<string> Urls = new List<string>();
		public bool Quiet;
		
		public LoggingSettings Logging = new LoggingSettings();
		public class LoggingSettings
		{
			public bool Debug;
			public string WriteTo;
			public string AppendTo;
		}
		
		public HttpSettings Http = new HttpSettings();
		public class HttpSettings
		{
			public HttpSettings()
			{
				UserAgent = "Wget/{0} (nutools)".With(Assembly.GetEntryAssembly().GetName().Version);
			}
			public string UserAgent;
		}
		
		public DownloadSettings Download = new DownloadSettings();
		public class DownloadSettings
		{
			public string OutputDocument;
			public ProgressType ProgressType;
		}
	}
}

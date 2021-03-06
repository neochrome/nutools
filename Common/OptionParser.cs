using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NuTools.Common
{
	public class OptionParser : OptionGroup
	{
		public void In(string groupName, Action<ICanBuildOptionGroup> optionsFor)
		{
			var group = new OptionGroup();
			optionsFor(group);
			groups.Add(groupName, group);
		}

		public void Parse(string[] args)
		{
			var expr = new Regex(@"^--(?<name>[a-zA-Z0-9]+[\-a-zA-Z0-9]*)(=(?<value>\w+))?|-(?<short_name>[a-zA-Z0-9]+)$", RegexOptions.ExplicitCapture);

			for (var i = 0; i < args.Length; i++)
			{
				var match = expr.Match(args[i]);
				if (match.Success)
				{
					if (match.Groups["name"].Success)
					{
						var option = FindOption(match.Groups["name"].Value);
						if (match.Groups["value"].Success)
							option.Receive(match.Groups["value"].Value);
						else
							option.ReceiveDefault();
					}
					else
					{
						var names = match.Groups["short_name"].Value.Select(name => name.ToString());
						names.TakeWhile(n => n != names.Last()).Each(n => FindOption(n).ReceiveDefault());

						var option = FindOption(names.Last());
						if (option.HasArgument && i + 1 < args.Length)
							option.Receive(args[++i]);
						else if (!option.HasArgument)
							option.ReceiveDefault();
						else
							throw new OptionParserException("--{0}: Missing argument".With(option.Name));
					}
				}
				else
				{
					// Handle arguments
					FindOption(string.Empty).Receive(args[i]);
				}
			}
			AllOptions.Each(o => o.Finally());
		}

		public string Header = string.Empty;
		public string Footer = string.Empty;

		public void WriteUsage(TextWriter output)
		{
			WriteUsageHeader(output);
			WriteUsageOptions(output);
			WriteUsageFooter(output);
		}

		public void WriteVersionInfo(TextWriter output)
		{
			output.WriteLine(Assembly.GetEntryAssembly().FullName);
		}

		public void WriteUsageHeader(TextWriter output)
		{
			var applicationName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
			output.Write("Usage: {0} [OPTION]...", applicationName);
			output.Write(
				AllOptions
					.OfType<Argument>()
					.OrderByDescending(a => a.Required)
					.Select(a => a.Required ? a.Name : string.Format("[{0}]", a.Name) + (a.SupportsMultipleValues ? " ..." : "") )
					.Aggregate("", (acc, a) => acc + " " + a)
				);
			output.WriteLine();
			
			if (string.IsNullOrEmpty(Header)) return;
			output.WriteLine();
			output.WriteLine(Header);
		}

		public void WriteUsageOptions(TextWriter output)
		{
			var anyShortOptions = AllOptions.OfType<Option>().Any(o => o.ShortNameForUsage.Length > 0);
			var maxOptionNameLength = AllOptions.OfType<Option>().Max(o => o.NameForUsage.Length);

			var formatter = new FixedWidthFormatProvider();
			Action<Option> writeOption = o => {
				output.Write("  ");
				if (anyShortOptions)
				{
					output.Write(string.Format(formatter, "{0:2}", o.ShortNameForUsage));
					if (o.NameForUsage.Length > 0)
						output.Write(o.ShortNameForUsage.Length > 0 ? "," : " ");
				}
				output.WriteLine(string.Format(formatter, " {0:" + maxOptionNameLength + "} {1}", o.NameForUsage, o.DescriptionForUsage));
			};

			var describedArguments = AllOptions
				.OfType<Argument>()
				.Where(a => a.DescriptionForUsage.Length > 0)
				.Select(a => a.DescriptionForUsage);
			if (describedArguments.Any())
			{
				output.WriteLine();
				describedArguments.Each(output.WriteLine);
			}
			
			if (Options.OfType<Option>().Any())
			{
				output.WriteLine();
				Options.OfType<Option>().Each(writeOption);
			}

			groups
				.Each(g => {
				    output.WriteLine();
				    output.WriteLine("{0}:", g.Key);
				    g.Value.Options.OfType<Option>().Each(writeOption);
				});
		}

		public void WriteUsageFooter(TextWriter output)
		{
			if (string.IsNullOrEmpty(Footer)) return;
			output.WriteLine();
			output.WriteLine(Footer);
		}

		private OptionBase FindOption(string name)
		{
			var option = AllOptions.FirstOrDefault(o => o.Match(name));
			if(option == null)
				throw new OptionParserException("Unknown option: {0}".With(name));
			return option;
		}

		private IEnumerable<OptionBase> AllOptions { get { return Options.Union(groups.Values.SelectMany(g => g.Options)); } }

		private Dictionary<string, OptionGroup> groups = new Dictionary<string, OptionGroup>();
	}
}

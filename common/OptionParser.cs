using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class OptionParser : OptionGroup
	{
		public void In(string groupName, Action<IOptionGroup> optionsFor)
		{
			var group = new OptionGroup();
			optionsFor(group);
			groups.Add(groupName, group);
		}

		public bool Parse(string[] args)
		{
			var expr = new Regex(@"^--(?<name>[a-zA-Z0-9]+[\-a-zA-Z0-9]*)(=(?<value>\w+))?|-(?<short_name>[a-zA-Z0-9]+)$", RegexOptions.ExplicitCapture);
			var allParsedOk = true;

			for (var i = 0; i < args.Length; i++)
			{
				var match = expr.Match(args[i]);
				if (match.Success)
				{
					if (match.Groups["name"].Success)
					{
						var option = FindOption(match.Groups["name"].Value);
						if (match.Groups["value"].Success)
							allParsedOk &= option.Receive(match.Groups["value"].Value);
						else
							allParsedOk &= option.ReceiveDefault();
					}
					else
					{
						var names = match.Groups["short_name"].Value.Select(name => name.ToString());
						names.TakeWhile(n => n != names.Last()).Each(n => FindOption(n).ReceiveDefault());

						var option = FindOption(names.Last());
						if (option.HasArgument && i + 1 < args.Length)
							allParsedOk &= option.Receive(args[++i]);
						else if (!option.HasArgument)
							allParsedOk &= option.Receive(true.ToString());
						else
							return false;//error
					}
				}
				else
				{
					allParsedOk &= FindOption(string.Empty).Receive(args[i]);
				}
			}
			return allParsedOk && Options.Where(o => o.Required).All(o => o.Parsed);
		}

		public string Header = string.Empty;
		public string Footer = string.Empty;

		public string Summary()
		{
			return "";
		}

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

		private void WriteUsageHeader(TextWriter output)
		{
			var applicationName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
			output.Write("Usage: {0} [OPTION]...", applicationName);
			output.Write(
				AllOptions
					.OfType<Argument>()
					.OrderByDescending(a => a.Required)
					.Select(a => a.Required ? a.Name : string.Format("[{0}]", a.Name))
					.Aggregate("", (acc, a) => acc + " " + a)
				);

			if (string.IsNullOrEmpty(Header)) return;
			output.WriteLine();
			output.WriteLine(Header);
		}

		private void WriteUsageOptions(TextWriter output)
		{
			var anyShortOptions = AllOptions.OfType<Option>().Any(o => o.ShortNameForUsage.Length > 0);
			var maxOptionNameLength = AllOptions.OfType<Option>().Max(o => o.NameForUsage.Length);

			var formatter = new FixedWithFormatProvider();
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

			output.WriteLine();
			Options.OfType<Option>().Each(writeOption);

			groups
				.Each(g => {
				    output.WriteLine();
				    output.WriteLine("{0}:", g.Key);
				    g.Value.Options.OfType<Option>().Each(writeOption);
				});
		}

		private void WriteUsageFooter(TextWriter output)
		{
			if (string.IsNullOrEmpty(Footer)) return;
			output.WriteLine();
			output.WriteLine(Footer);
		}

		private OptionBase FindOption(string name)
		{
			return AllOptions.FirstOrDefault(o => o.Match(name)) ?? OptionBase.Default;
		}

		private IEnumerable<OptionBase> AllOptions { get { return Options.Union(groups.Values.SelectMany(g => g.Options)); } }

		private Dictionary<string, OptionGroup> groups = new Dictionary<string, OptionGroup>();
	}
}

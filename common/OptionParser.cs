using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class OptionParser : OptionGroup
	{
		public OptionParser() { }

		public void In(string groupName, Action<IOptionGroup> optionsFor)
		{
			var group = new OptionGroup();
			optionsFor(group);
			groups.Add(groupName, group);
			Options.AddRange(group.Options);
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

		public string Banner = string.Empty;

		public string Summary()
		{
			var summary = new StringBuilder();
			summary.Append(Banner);
			//options.Where(x => x.HasValueName).Each(x => summary.AppendFormat(x.Required ? " {0}" : " [{0}]", x.ValueName));
			summary.AppendLine();
			return summary.ToString();
		}

		public void WriteDescriptions(System.IO.TextWriter output)
		{
			var maxOptionNameLength = 
				Options.OfType<Option>()
				.Union(groups.Values.SelectMany(g => g.Options.OfType<Option>()))
				.Max(o => o.NameForDescription);

			Options.OfType<Option>()
				.Each(o => output.WriteLine("{0}, {1} {2}", o.ShortNameForDescription, o.NameForDescription, o.Description));
			groups
				.Each(g => {
					output.WriteLine();
 					output.WriteLine("{0}:", g.Key);
					g.Value.Options.OfType<Option>()
						.Each(o => output.WriteLine("{0}, {1} {2}", o.ShortNameForDescription, o.NameForDescription, o.Description));
				});
		}

		private OptionBase FindOption(string name)
		{
			return Options.FirstOrDefault(o => o.Match(name)) ?? OptionBase.Default;
		}

		private Dictionary<string, OptionGroup> groups = new Dictionary<string, OptionGroup>();
	}
}

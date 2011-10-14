using System;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class Option : OptionBase, INoArg
	{
		public string Name;
		public string ShortName;

		public override bool Match(string argument)
		{
			return Name == argument || ShortName == argument;
		}

		public override bool ReceiveDefault()
		{
			return Receive(true.ToString());
		}

		public override bool Receive(string value)
		{
			action();
			Parsed = true;
			return true;
		}

		public void Do(Action action)
		{
			this.action = action;
		}

		public virtual string NameForUsage { get { return string.IsNullOrEmpty(Name) ? "" : "--" + Name; } }
		public virtual string ShortNameForUsage { get { return string.IsNullOrEmpty(ShortName) ? "" : "-" + ShortName; } }
		public virtual string DescriptionForUsage { get { return Description; } }

		private Action action = () => { };
	}
}

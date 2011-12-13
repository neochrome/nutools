using System;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class Option : OptionBase
	{
		public string ShortName;

		public override bool Match(string argument)
		{
			return Name == argument || ShortName == argument;
		}

		public override void ReceiveDefault()
		{
			Receive(true.ToString());
		}

		public override void Receive(string value)
		{
			Parsed = true;
			action();
		}

		public void Do(Action action)
		{
			this.action = action;
		}

		public override void Finally()
		{
			if(Required && !Parsed)
				throw new OptionParserException("Missing required option: --{0}".With(Name));
		}

		public virtual string NameForUsage { get { return string.IsNullOrEmpty(Name) ? "" : "--" + Name; } }
		public virtual string ShortNameForUsage { get { return string.IsNullOrEmpty(ShortName) ? "" : "-" + ShortName; } }

		private Action action = () => { };
	}
}

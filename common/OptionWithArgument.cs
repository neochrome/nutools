using System;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class OptionWithArgument<T> : Option, IArg<T>
	{
		public string ArgumentName;
		public override bool HasArgument { get { return true; } }

		public override bool ReceiveDefault()
		{
			return false;
		}

		public override bool Receive(string value)
		{
			receivedValue = (T)Convert.ChangeType(value, typeof(T));
			Parsed = true;
			return true;
		}

		public void Do(Action<T> action)
		{
			this.action = action;
		}

		public override void Tell()
		{
			action(receivedValue);
		}

		public override string NameForUsage { get { return base.NameForUsage + "=" + ArgumentName; } }
		public override string DescriptionForUsage { get { return Description.Replace("{ARG}", ArgumentName); } }

		private T receivedValue;
		private Action<T> action = v => { };
	}
}

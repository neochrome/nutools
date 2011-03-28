using System;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class Argument<T> : OptionBase, IArg<T>
	{
		public string Name;

		public override bool Match(string argument)
		{
			return argument == "";
		}

		public override bool ReceiveDefault()
		{
			return false;
		}

		public override bool Receive(string value)
		{
			action((T)Convert.ChangeType(value, typeof(T)));
			Parsed = true;
			return true;
		}

		public void Do(Action<T> action)
		{
			this.action = action;
		}

		private Action<T> action = v => { };
	}
}

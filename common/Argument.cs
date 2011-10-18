using System;
using System.Collections.Generic;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public abstract class Argument : OptionBase
	{
		public string Name;

		public override bool Match(string argument)
		{
			return argument == "" && !Parsed;
		}

		public override bool ReceiveDefault()
		{
			return false;
		}

		public abstract bool SupportsMultipleValues { get; }
	}

	public class Argument<T> : Argument, IArg<T>
	{
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

		public override bool SupportsMultipleValues { get { return false; } }

		private T receivedValue;
		private Action<T> action = v => { };
	}

	public class Arguments<T> : Argument, IArgs<T>
	{
		public override bool Receive(string value)
		{
			receivedValues.Add((T)Convert.ChangeType(value, typeof(T)));
			return true;
		}

		public void Do(Action<T[]> action)
		{
			this.action = action;
		}

		public override void Tell()
		{
			action(receivedValues.ToArray());
		}

		public override bool SupportsMultipleValues { get { return true; } }

		private readonly List<T> receivedValues = new List<T>();
		private Action<T[]> action = v => { };
	}
}

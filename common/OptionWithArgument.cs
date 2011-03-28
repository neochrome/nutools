﻿using System;
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
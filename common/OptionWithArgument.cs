using System;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class OptionWithArgument<T> : Option, IArg<T>
	{
		public string ArgumentName;
		public string ParseErrorMessage = "--{0}: Received an invalid argument '{1}'";

		public override bool HasArgument { get { return true; } }

		public override bool ReceiveDefault()
		{
			return false;
		}

		public override bool Receive(string value)
		{
			try
			{
				var typeOfT = typeof(T);
				if(typeOfT.IsEnum)
				{
					receivedValue = (T)Enum.Parse(typeOfT, value, true);
				}
				else
				{
					receivedValue = (T)Convert.ChangeType(value, typeof(T));
				}
					
				Parsed = true;
				return true;
			}
			catch(Exception ex)
			{
				throw new OptionParserException(ParseErrorMessage.With(Name, value), ex);
			}
		}
		
		IArg<T> IArg<T>.WithParseErrorMessage(string message)
		{
			this.ParseErrorMessage = message;
			return this;
		}

		public void Do(Action<T> action)
		{
			this.action = action;
		}

		public override void Tell()
		{
			if (Parsed)
                action(receivedValue);
		}

		public override string NameForUsage { get { return base.NameForUsage + "=" + ArgumentName; } }
		public override string DescriptionForUsage { get { return Description.Replace("{ARG}", ArgumentName); } }

		private T receivedValue;
		private Action<T> action = v => { };
	}
}

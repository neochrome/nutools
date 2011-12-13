using System;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public class OptionWithArgument<T> : Option, IArg<T>
	{
		public string ArgumentName;
		public string ParseErrorMessage = "--{0}: Received an invalid argument '{1}'";

		public override bool HasArgument { get { return true; } }

		public override void ReceiveDefault() { }

		public override void Receive(string value)
		{
			T parsedValue;
			try
			{
				var typeOfT = typeof(T);
				if(typeOfT.IsEnum)
				{
					parsedValue = (T)Enum.Parse(typeOfT, value, true);
				}
				else
				{
					parsedValue = (T)Convert.ChangeType(value, typeof(T));
				}
					
				Parsed = true;
			}
			catch(Exception ex)
			{
				throw new OptionParserException(ParseErrorMessage.With(Name, value), ex);
			}
			action(parsedValue);
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

		public override void Finally()
		{
			if(Required && !Parsed)
				throw new OptionParserException("Missing required option: --{0}".With(Name));
		}

		public override string NameForUsage { get { return base.NameForUsage + "=" + ArgumentName; } }
		public override string DescriptionForUsage { get { return Description.Replace("{ARG}", ArgumentName); } }

		private Action<T> action = v => { };
	}
}

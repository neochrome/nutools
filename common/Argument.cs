using System;
using System.Collections.Generic;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public abstract class Argument : OptionBase
	{
		public string ParseErrorMessage = "{0}: Received an invalid argument '{1}'";

		public override bool Match(string argument)
		{
			return argument == "" && !Parsed;
		}

		public override void ReceiveDefault() {	}

		public abstract bool SupportsMultipleValues { get; }
		
		public override string DescriptionForUsage { get { return string.IsNullOrEmpty(Description) ? "" : string.Format(Description, Name); } }
	}

	public class Argument<T> : Argument, IArg<T>
	{
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
				throw new OptionParserException("Missing argument: {0}".With(Name));
		}

		public override bool SupportsMultipleValues { get { return false; } }

		private Action<T> action = v => { };
	}

	public class Arguments<T> : Argument, IArgs<T>
	{
		public override void Receive(string value)
		{
			try
			{
				var typeOfT = typeof(T);
				if(typeOfT.IsEnum)
				{
					receivedValues.Add((T)Enum.Parse(typeOfT, value, true));
				}
				else
				{
					receivedValues.Add((T)Convert.ChangeType(value, typeof(T)));
				}
			}
			catch(Exception ex)
			{
				throw new OptionParserException(ParseErrorMessage.With(Name, value), ex);
			}
		}

		IArgs<T> IArgs<T>.WithParseErrorMessage(string message)
		{
			this.ParseErrorMessage = message;
			return this;
		}

		public void Do(Action<T[]> action)
		{
			this.action = action;
		}

		public override void Finally()
		{
			action(receivedValues.ToArray());
		}

		public override bool SupportsMultipleValues { get { return true; } }

		private readonly List<T> receivedValues = new List<T>();
		private Action<T[]> action = v => { };
	}
}

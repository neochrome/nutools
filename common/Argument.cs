using System;
using System.Collections.Generic;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public abstract class Argument : OptionBase
	{
		public string Name;
		public string ParseErrorMessage = "{0}: Received an invalid argument '{1}'";

		public override bool Match(string argument)
		{
			return argument == "" && !Parsed;
		}

		public override bool ReceiveDefault()
		{
			return false;
		}

		public abstract bool SupportsMultipleValues { get; }
		
		public override string DescriptionForUsage { get { return string.IsNullOrEmpty(Description) ? "" : string.Format(Description, Name); } }
	}

	public class Argument<T> : Argument, IArg<T>
	{
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
					
				return true;
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

		public override void Tell()
		{
			action(receivedValues.ToArray());
		}

		public override bool SupportsMultipleValues { get { return true; } }

		private readonly List<T> receivedValues = new List<T>();
		private Action<T[]> action = v => { };
	}
}

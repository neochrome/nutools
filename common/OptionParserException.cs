
using System;
using NuTools.Common.OptionParserSyntax;
namespace NuTools.Common
{
	public class OptionParserException : Exception
	{
		public OptionParserException() : base() {}
		public OptionParserException(string message) : base(message) { }
		public OptionParserException(string message, Exception inner) : base(message, inner) { }
	}
}

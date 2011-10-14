using System;

namespace NuTools.Common
{
	public class FixedWithFormatProvider : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType)
		{
			return formatType == typeof(ICustomFormatter) ? this : null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			int justification;
			int.TryParse(format, out justification);
			var argAsString = arg.ToString();
			var padding = Math.Max(0, Math.Abs(justification) - argAsString.Length);
			if(justification > 0)
				return argAsString + new string(' ', padding);
			return new string(' ', padding) + argAsString;
		}
	}
}
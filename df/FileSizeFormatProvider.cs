using System;
using System.Globalization;

namespace NuTools.Df
{
	public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
				return this;
			return null;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (format == null || !format.StartsWith(formatFlag))
				return DefaultFormat(format, arg, formatProvider);

			var size = Convert.ToDecimal(arg);
			var suffix = "B";

			if (size >= oneTeraByte)
			{
				size = size / oneTeraByte;
				suffix = "T";
			}
			else if (size >= oneGigaByte)
			{
				size = size / oneGigaByte;
				suffix = "G";
			}
			else if (size >= oneMegaByte)
			{
				size = size / oneMegaByte;
				suffix = "M";
			} 
			else if (size >= oneKiloByte)
			{
				size = size / 1024;
				suffix = "K";
			}

			var sizeString = string.Format("{0:0.0}{1}", size, suffix, CultureInfo.InvariantCulture);
			return sizeString.Replace(',', '.');
		}

		private string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
		{
			var formattable = arg as IFormattable;
			if (formattable != null)
				return formattable.ToString(format, formatProvider);
			return arg.ToString();
		}

		private const decimal oneKiloByte = 1024M;
		private const decimal oneMegaByte = oneKiloByte * 1024M;
		private const decimal oneGigaByte = oneMegaByte * 1024M;
		private const decimal oneTeraByte = oneGigaByte * 1024M;
		private const string formatFlag = "fs";
	}
}
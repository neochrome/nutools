using System;
namespace NuTools.Common
{
	public static class StringExtensions
	{
		public static string With(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static string With(this string format, IFormatProvider provider, params object[] args)
		{
			return string.Format(provider, format, args);
		}
	}
}


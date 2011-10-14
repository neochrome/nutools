using Cone;
using NuTools.Common;

namespace NuTools.Specs
{
	[Describe(typeof(FixedWithFormatProvider))]
	public class FixedWithFormatProviderSpecs
	{
		[Row("{0:10}", "",		 "          ")]
		[Row("{0:10}", "monkey", "monkey    ")]
		[Row("{0:10}", "12345",  "12345     ")]
		[Row("{0:10}", "somethinglong", "somethinglong")]
		public void left_justification(string format, object value, string formatted)
		{
			Verify.That(() => string.Format(new FixedWithFormatProvider(), format, value) == formatted);
		}

		[Row("{0:-10}", "",		  "          ")]
		[Row("{0:-10}", "monkey", "    monkey")]
		[Row("{0:-10}", "12345",  "     12345")]
		[Row("{0:-10}", "somethinglong", "somethinglong")]
		public void right_justification(string format, object value, string formatted)
		{
			Verify.That(() => string.Format(new FixedWithFormatProvider(), format, value) == formatted);
		}
	}
}
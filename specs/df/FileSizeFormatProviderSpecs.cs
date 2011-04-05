using Cone;
using NuTools.Df;

namespace NuTools.Specs.df
{
	[Describe(typeof(FileSizeFormatProvider))]
	public class FileSizeFormatProviderSpecs
	{
		public void requires_format_flag()
		{
			var provider = new FileSizeFormatProvider();
			var result = string.Format(provider, "{0:fs}", 1M);

			Verify.That(() => result == "1.0B");
		}

		[Context("Converts")]
		public class Converts
		{
			[BeforeAll]
			public void before_all()
			{
				oneKiloByte = 1024M;
				oneMegaByte = oneKiloByte * 1024M;
				oneGigaByte = oneMegaByte * 1024M;
				oneTeraByte = oneGigaByte * 1024M;
			}

			public void to_default_format_for_the_input_when_format_is_unknown()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:x}", 10);
	
				Verify.That(() => result == "a");
			}

			public void to_string_when_original_input_is_string()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0}", "foo");

				Verify.That(() => result == "foo");
			}

			public void to_result_with_dot_as_decimal_seperator()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", 1536);

				Verify.That(() => result == "1.5K");
			}

			public void to_result_with_one_decimal()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", 1664);
				
				Verify.That(() => result == "1.6K");
			}

			public void to_bytes()
			{
				long oneByte = 1;
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", oneByte);

				Verify.That(() => result == "1.0B");
			}

			public void to_kilo_bytes()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", oneKiloByte);

				Verify.That(() => result == "1.0K");
			}

			public void to_mega_bytes()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", oneMegaByte);

				Verify.That(() => result == "1.0M");
			}

			public void to_giga_bytes()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", oneGigaByte);

				Verify.That(() => result == "1.0G");
			}

			public void to_tera_bytes()
			{
				var provider = new FileSizeFormatProvider();
				var result = string.Format(provider, "{0:fs}", oneTeraByte);

				Verify.That(() => result == "1.0T");
			}

			private decimal oneKiloByte;
			private decimal oneMegaByte;
			private decimal oneGigaByte;
			private decimal oneTeraByte;
		}
	}
}
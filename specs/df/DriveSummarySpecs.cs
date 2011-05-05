using System.Linq;
using NuTools.Df;
using Cone;
using System.Collections.Generic;

namespace NuTools.Specs
{
	[Describe(typeof(DriveSummary))]
	public class DriveSummarySpecs
	{
		[Context("renders")]
		public class Rendering
		{
			public void drive_letter()
			{
				var drives = new List<IDrive> { new Drive(@"C:\") };
				var summary = new DriveSummary(drives);
				
				var output = summary.Render();
				
				Verify.That(() => output.Contains(@"C:\"));
			}

			public void file_system_format()
			{
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32") };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains("FAT32"));
			}

			public void total_size_in_bytes()
			{
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", 1024) };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains("1024"));
			}

			public void total_free_space_in_bytes()
			{
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", 1024, 256) };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains("256"));
			}

			public void total_used_space_in_bytes()
			{
				var totalSize = 1024;
				var free = 256;
				var expectedUsedSpace = totalSize - free;
				
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", totalSize, free) };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains(expectedUsedSpace.ToString()));
			}

			public void total_size_including_weight()
			{
				var oneMegaByte = 1024 * 1024;
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", oneMegaByte) };
				var summary = new DriveSummary(drives, new FileSizeFormatProvider());

				var output = summary.Render();

				Verify.That(() => output.Contains("1.0M"));
			}

			public void total_free_space_including_weight()
			{
				var oneMegaByte = 1024 * 1024;
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", 0, oneMegaByte) };
				var summary = new DriveSummary(drives, new FileSizeFormatProvider());

				var output = summary.Render();

				Verify.That(() => output.Contains("1.0M"));
			}

			public void total_used_space_including_weight()
			{
				var oneMegaByte = 1024 * 1024;
				var free = oneMegaByte;
				var totalSize = oneMegaByte + oneMegaByte + oneMegaByte;
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", totalSize, free) };
				var summary = new DriveSummary(drives, new FileSizeFormatProvider());

				var output = summary.Render();

				Verify.That(() => output.Contains("2.0M"));
			}

			public void headers()
			{
				var summary = new DriveSummary(new Drive[] { new Drive(@"C:\") });
				var output = summary.Render();
				Verify.That(() => output.Contains("Drive"));
				Verify.That(() => output.Contains("Type"));
				Verify.That(() => output.Contains("Size"));
				Verify.That(() => output.Contains("Used"));
				Verify.That(() => output.Contains("Avail"));
			}
		}
	}
}
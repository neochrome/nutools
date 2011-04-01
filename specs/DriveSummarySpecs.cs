﻿using System.Linq;
using NuTools.Df;
using Cone;
using System.Collections.Generic;

namespace NuTools.Specs
{
	[Describe(typeof(DriveSummary))]
	public class DriveSummarySpecs
	{
		[Context("rendering")]
		public class Rendering
		{
			public void should_display_drive_letter()
			{
				var drives = new List<IDrive> { new Drive(@"C:\") };
				var summary = new DriveSummary(drives);
				
				var output = summary.Render();
				
				Verify.That(() => output.Contains(@"C:\"));
			}

			public void should_display_file_system_format()
			{
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32") };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains("FAT32"));
			}

			public void should_display_total_size_in_bytes()
			{
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", 1024) };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains("1024"));
			}

			public void should_display_total_free_space_in_bytes()
			{
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", 1024, 256) };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains("256"));
			}

			public void should_display_total_used_space_in_bytes()
			{
				var totalSize = 1024;
				var free = 256;
				var expectedUsedSpace = totalSize - free;
				
				var drives = new List<IDrive> { new Drive(@"C:\", "FAT32", totalSize, free) };
				var summary = new DriveSummary(drives);

				var output = summary.Render();

				Verify.That(() => output.Contains(expectedUsedSpace.ToString()));
			}
		}
	}
}
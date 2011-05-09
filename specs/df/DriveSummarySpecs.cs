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
			[Context("information")]
			public class Information
			{
				[BeforeAll]
				public void Context()
				{
					summaryOutput = "";
					drives = new List<IDrive> { new Drive(@"C:\", "FAT32", 1024, 1024 / 4) };
					summary = new DriveSummary(drives);

					summaryOutput = summary.Render();
				}

				public void headers()
				{
					Verify.That(() => summaryOutput.Contains("Drive"));
					Verify.That(() => summaryOutput.Contains("Type"));
					Verify.That(() => summaryOutput.Contains("Size"));
					Verify.That(() => summaryOutput.Contains("Used"));
					Verify.That(() => summaryOutput.Contains("Avail"));
				}

				public void drive_letter()
				{
					Verify.That(() => summaryOutput.Contains(@"C:\"));
				}

				public void file_system_format()
				{
					Verify.That(() => summaryOutput.Contains("FAT32"));
				}

				private string summaryOutput;
				private IList<IDrive> drives;
				private DriveSummary summary;
			}

			[Context("as bytes")]
			public class AsBytes
			{
				[BeforeAll]
				public void Context()
				{
					summaryOutput = "";
					drives = new List<IDrive> { new Drive("C:\\", "FAT32", 1024, 1024 / 4) };
					summary = new DriveSummary(drives);

					summaryOutput = summary.Render();
				}

				public void total_size()
				{
					Verify.That(() => summaryOutput.Contains("1024"));
				}

				public void total_free_space()
				{
					Verify.That(() => summaryOutput.Contains("768"));
				}

				public void total_used_space()
				{
					Verify.That(() => summaryOutput.Contains("256"));
				}

				private string summaryOutput;
				private IList<IDrive> drives;
				private DriveSummary summary;
			}

			[Context("as human readable")]
			public class AsHumanReadable
			{
				[BeforeAll]
				public void Context()
				{
					var oneMegaByte = 1024 * 1024;

					summaryOutput = "";
					drives = new List<IDrive> { new Drive("C:\\", "FAT32", oneMegaByte * 10, (oneMegaByte * 10) / 4) };
					summary = new DriveSummary(drives, new FileSizeFormatProvider());

					summaryOutput = summary.Render();
				}
	
				public void total_size()
				{
					Verify.That(() => summaryOutput.Contains("10.0M"));
				}

				public void total_free_space()
				{
					Verify.That(() => summaryOutput.Contains("2.5M"));
				}

				public void total_used_space()
				{
					Verify.That(() => summaryOutput.Contains("7.5M"));
				}

				private string summaryOutput;
				private IList<IDrive> drives;
				private DriveSummary summary;
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuTools.Df
{
	public class DriveSummary
	{
		public DriveSummary() { }

		public DriveSummary(IEnumerable<IDrive> drives) : this()
		{
			this.drives = drives;
			this.formatDefinition = "{0,-5} {1,-5} {2,14} {3,14} {4,14}";
			this.headerFormatDefinition = formatDefinition;
			this.format = (drive) => {
				return string.Format(formatDefinition, drive.Letter, drive.Format, drive.Size, drive.Free, drive.Used);
			};
		}

		public DriveSummary(IEnumerable<IDrive> drives, FileSizeFormatProvider formatProvider) : this(drives)
		{
			this.headerFormatDefinition = "{0,-5} {1,-5} {2,6} {3,6} {4,6}";
			this.formatDefinition = "{0,-5} {1,-5} {2,6:fs} {3,6:fs} {4,6:fs}";
			this.format = (drive) =>
			{
				return string.Format(formatProvider, formatDefinition, drive.Letter, drive.Format, drive.Size, drive.Free, drive.Used);
			};
		}

		public string Render()
		{
			var summary = new StringBuilder();
			summary.AppendLine(string.Format(headerFormatDefinition, "Drive", "Type", "Size", "Used", "Avail"));
			foreach (var drive in drives)
				summary.AppendLine(format(drive));
			return summary.ToString();
		}

		private Func<IDrive, string> format;
		private readonly IEnumerable<IDrive> drives;
		private string formatDefinition;
		private string headerFormatDefinition;
	}
}
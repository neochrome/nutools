using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuTools.Df
{
	public class DriveSummary
	{
		public DriveSummary(IEnumerable<IDrive> drives)
		{
			this.drives = drives;
			this.formatDefinition = "{0,-5} {1,-5} {2,14} {3,14} {4,14} {5,4}%";
			this.headerFormatDefinition = formatDefinition;
			this.format = (drive) => {
				return string.Format(
					formatDefinition,
					drive.Letter,
					drive.Format,
					drive.Size,
					drive.Used,
					drive.Free,
					PercentUsedOf(drive)
				);
			};
		}

		public DriveSummary(IEnumerable<IDrive> drives, FileSizeFormatProvider formatProvider) : this(drives)
		{
			this.headerFormatDefinition = "{0,-5} {1,-5} {2,6} {3,6} {4,6} {5,4}%";
			this.formatDefinition = "{0,-5} {1,-5} {2,6:fs} {3,6:fs} {4,6:fs} {5,4}%";
			this.format = (drive) => {
				return string.Format(formatProvider, formatDefinition,
					drive.Letter,
					drive.Format,
					drive.Size,
					drive.Used,
					drive.Free,
					PercentUsedOf(drive)
				);
			};
		}

		public string Render()
		{
			var summary = new StringBuilder();
			summary.AppendLine(string.Format(headerFormatDefinition, "Drive", "Type", "Size", "Used", "Avail", "Use"));
			foreach (var drive in drives)
				summary.AppendLine(format(drive));
			return summary.ToString();
		}

		private int PercentUsedOf(IDrive drive)
		{
			return (drive.Size <= 0 || drive.Used <= 0) ? 0 : (int)Math.Round((drive.Used / (float)drive.Size) * 100);
		}

		private Func<IDrive, string> format;
		private readonly IEnumerable<IDrive> drives;
		private string formatDefinition;
		private string headerFormatDefinition;
	}
}
using System.Collections.Generic;
using System.Text;
using System;

namespace NuTools.Df
{
	public class DriveSummary
	{
		public DriveSummary(IEnumerable<IDrive> drives)
		{
			this.drives = drives;
			this.format = (drive) => { return string.Format("{0} {1} {2} {3} {4}", drive.Letter, drive.Format, drive.Size, drive.Free, drive.Used); };
		}

		public DriveSummary(IEnumerable<IDrive> drives, FileSizeFormatProvider formatProvider) : this(drives)
		{
			this.format = (drive) => { return string.Format(formatProvider, "{0} {1} {2:fs} {3:fs} {4:fs}", drive.Letter, drive.Format, drive.Size, drive.Free, drive.Used); };
		}

		public string Render()
		{
			var summary = new StringBuilder();
			foreach (var drive in drives)
				summary.AppendLine(format(drive));
			return summary.ToString();
		}

		private Func<IDrive, string> format;
		private readonly IEnumerable<IDrive> drives;
	}
}
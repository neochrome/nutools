using System.Collections.Generic;
using System.Text;

namespace NuTools.Df
{
	public class DriveSummary
	{
		public DriveSummary(IEnumerable<IDrive> drives)
		{
			this.drives = drives;
		}

		public string Render()
		{
			var summary = new StringBuilder();
			foreach (var drive in drives)
			{
				var diskOut = string.Format("{0} {1} {2} {3} {4}", drive.Letter, drive.Format, drive.Size, drive.Free, drive.Avalible);
				summary.AppendLine(diskOut);
			}
			return summary.ToString();
		}

		private readonly IEnumerable<IDrive> drives;
	}
}
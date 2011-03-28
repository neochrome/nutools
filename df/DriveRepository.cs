using System.IO;
using System.Collections.Generic;

namespace NuTools.Df
{
	public class DriveRepository
	{
		public IEnumerable<IDrive> GetDrives()
		{
			var drives = new List<IDrive>();
			
			foreach (DriveInfo info in DriveInfo.GetDrives())
			{
				var drive = Drive.LoadFrom(info);
				if (drive is NotSupportedDrive)
					continue;
				
				drives.Add(Drive.LoadFrom(info));
			}

			return drives;
		}

	}
}
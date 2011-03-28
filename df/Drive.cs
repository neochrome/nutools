using System.IO;

namespace NuTools.Df
{
	// Todo: Get a hold of the disk number, eg. Disk 0
	public class Drive : IDrive
	{
		public static IDrive LoadFrom(DriveInfo info)
		{
			if (info.DriveType != DriveType.Fixed && info.DriveType != DriveType.Removable)
				return new NotSupportedDrive();

			var drive = new Drive
			{
				Letter = info.Name,
				Format = info.DriveFormat,
				Size = info.TotalSize,
				Free = info.TotalFreeSpace,
				Avalible = info.AvailableFreeSpace
			};
			return drive;
		}

		public string Letter { get; private set; }
		public string Format { get; private set; }
		public long Size { get; private set; }
		public long Free { get; private set; }
		public long Avalible { get; private set; }
	}
}
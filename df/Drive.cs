using System.IO;

namespace NuTools.Df
{
	// Todo: Get a hold of the disk number, eg. Disk 0
	public class Drive : IDrive
	{
        public Drive()
        {
            Letter = "n/a";
            Format = "n/a";
            Size = 0;
            Free = 0;
        }

		public Drive(string letter) : this() { Letter = letter; }
		public Drive(string letter, string format) : this() { Letter = letter; Format = format; }
		public Drive(string letter, string format, long size) : this() { Letter = letter; Format = format; Size = size; }
		public Drive(string letter, string format, long size, long free) : this() { Letter = letter; Format = format; Size = size; Free = free; }

		public static IDrive LoadFrom(DriveInfo info)
		{
			if (!Supported(info))
				return new NotSupportedDrive();

            var drive = new Drive { Letter = info.Name };
            if (info.IsReady)
            {
                drive = new Drive
                {
                    Letter = info.Name,
                    Format = info.DriveFormat,
                    Size = info.TotalSize,
                    Free = info.TotalFreeSpace,
                };
            }
			return drive;
		}

        private static bool Supported(DriveInfo info)
        {
            return info.DriveType == DriveType.Fixed || info.DriveType == DriveType.Removable;
        }

		public string Letter { get; private set; }
		public string Format { get; private set; }
		public long Size { get; private set; }
		public long Free { get; private set; }
		public long Used { get { return Size - Free; } }
	}
}
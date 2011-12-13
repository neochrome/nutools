using System.IO;

namespace NuTools.Df
{
	public class Drive : IDrive
	{
        private Drive()
        {
            Letter = "n/a";
            Format = "n/a";
            Size = 0;
            Free = 0;
        }

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
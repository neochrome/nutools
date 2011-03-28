namespace NuTools.Df
{
	public class NotSupportedDrive : IDrive
	{
		public string Letter { get { return "n/a"; } }
		public string Format { get { return "n/a"; } }
		public long Size { get { return 0; } }
		public long Free { get { return 0; } }
		public long Avalible { get { return 0; } }
	}
}
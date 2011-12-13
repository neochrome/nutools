namespace NuTools.Df
{
	public interface IDrive
	{
		string Letter { get; }
		string Format { get; }
		long Size { get; }
		long Free { get; }
		long Used { get; }
	}
}
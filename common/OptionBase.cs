namespace NuTools.Common
{
	public abstract class OptionBase
	{
		public static readonly OptionBase Default = new Option();
		public string Description;
		public bool Parsed = false;
		public bool Required = false;
		public abstract bool Match(string argument);
		public abstract bool ReceiveDefault();
		public abstract bool Receive(string value);
		public virtual bool HasArgument { get { return false; } }
		public abstract void Tell();
	}
}

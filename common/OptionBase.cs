namespace NuTools.Common
{
	public abstract class OptionBase
	{
		public string Name;
		public string Description;
		public bool Parsed = false;
		public bool Required = false;
		public abstract bool Match(string argument);
		public abstract void ReceiveDefault();
		public abstract void Receive(string value);
		public virtual bool HasArgument { get { return false; } }
		public abstract void Finally();
		public virtual string DescriptionForUsage { get { return Description; } }
	}
}

using NuTools.Common;

namespace NuTools
{
	interface ICommand
	{
		void WithOptions(OptionParser opts);
		int Execute();
	}
}
using System;

namespace NuTools.Common.OptionParserSyntax
{
	public interface IArg<T>
	{
		void Do(Action<T> action);
	}

	public interface IArgs<T>
	{
		void Do(Action<T[]> action);
	}

	public interface IArg
	{
		IArg<T> Arg<T>(string name, string description);
		IArgs<T> Args<T>(string name, string description);
	}

	public interface INoArg
	{
		void Do(Action action);
	}

	public interface IOption : INoArg
	{
		IArg<T> WithArg<T>(string name);
	}

	public interface IOptionGroup : IArg
	{
		IOption On(string name, char shortName, string description);
		IOption On(char shortName, string description);
		IOption On(string name, string description);
		IArg Required { get; }
	}
}

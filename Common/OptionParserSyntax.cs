using System;

namespace NuTools.Common.OptionParserSyntax
{
	public interface IArg<T>
	{
		IArg<T> WithParseErrorMessage(string message);
		void Do(Action<T> action);
	}

	public interface IArgs<T>
	{
		IArgs<T> WithParseErrorMessage(string message);
		void Do(Action<T[]> action);
	}

	public interface ICanBuildArgument
	{
		IArg<T> Arg<T>(string name, string description);
		IArgs<T> Args<T>(string name, string description);
	}

	public interface ICanBuildWithAction
	{
		void Do(Action action);
	}

	public interface ICanBuildWithArgument : ICanBuildWithAction
	{
		IArg<T> WithArg<T>(string name);
	}

	public interface ICanBuildOption
	{
		ICanBuildWithArgument On(string name, char shortName, string description);
		ICanBuildWithArgument On(char shortName, string description);
		ICanBuildWithArgument On(string name, string description);
	}

	public interface ICanBuildRequiredArgument
	{
		ICanBuildArgument Required { get; }
	}
}
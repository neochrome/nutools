using System;
using System.Collections.Generic;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
	public interface ICanBuildOptionGroup : ICanBuildOption, ICanBuildArgument, ICanBuildRequiredArgument { }

	public class OptionGroup : ICanBuildOptionGroup, ICanBuildWithArgument, ICanBuildWithAction
	{
		public ICanBuildWithArgument On(string name, string description)
		{
			currentOption = new OptionSpec { Name = name, Description = description };
			return this;
		}

		public ICanBuildWithArgument On(char shortName, string description)
		{
			currentOption = new OptionSpec { ShortName = shortName, Description = description };
			return this;
		}

		public ICanBuildWithArgument On(string name, char shortName, string description)
		{
			currentOption = new OptionSpec { Name = name, ShortName = shortName, Description = description };
			return this;
		}

		void ICanBuildWithAction.Do(Action action)
		{
			var option = new Option
			{
				Name = currentOption.Name,
				ShortName = currentOption.ShortName.HasValue ? currentOption.ShortName.Value.ToString() : null,
				Description = currentOption.Description
			};
			option.Do(action);
			Options.Add(option);
			currentOption = null;
		}

		IArg<T> ICanBuildWithArgument.WithArg<T>(string name)
		{
			var option = currentOption.CreateOptionWithArg<T>(name);
			Options.Add(option);
			return option;
		}

		public ICanBuildArgument Required { get { currentOption = new OptionSpec { Required = true }; return this; } }

		public IArg<T> Arg<T>(string name, string description)
		{
			var argument = new Argument<T>
			{
				Name = name,
				Description = description,
				Required = currentOption != null ? currentOption.Required : false
			};
			return AddArgument(argument);
		}

		public IArgs<T> Args<T>(string name, string description)
		{
			var argument = new Arguments<T>
			{
				Name = name,
				Description = description,
				Required = false
			};
			
			return AddArgument(argument);
		}
		
		private T AddArgument<T>(T argument) where T: Argument
		{
			Options.Add(argument);
			currentOption = null;
			return argument;
		}

		public List<OptionBase> Options = new List<OptionBase>();

		private OptionSpec currentOption;

		private class OptionSpec
		{
			public string Name;
			public char? ShortName;
			public string Description;
			public bool Required;

			public OptionWithArgument<T> CreateOptionWithArg<T>(string argumentName)
			{
				var option = new OptionWithArgument<T>
				{
					Name = Name,
					ShortName = ShortName.HasValue ? ShortName.Value.ToString() : null,
					Description = Description,
					ArgumentName = argumentName,
				};
				return option;
			}
		}
	}

}
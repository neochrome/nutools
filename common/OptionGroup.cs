using System;
using System.Collections.Generic;
using NuTools.Common.OptionParserSyntax;

namespace NuTools.Common
{
		public class OptionGroup : IOptionGroup, IOption
	{
		public IOption On(string name, string description)
		{
			currentOption = new OptionSpec { Name = name, Description = description };
			return this;
		}

		public IOption On(char shortName, string description)
		{
			currentOption = new OptionSpec { ShortName = shortName, Description = description };
			return this;
		}

		public IOption On(string name, char shortName, string description)
		{
			currentOption = new OptionSpec { Name = name, ShortName = shortName, Description = description };
			return this;
		}

		void INoArg.Do(Action action)
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

		IArg<T> IOption.WithArg<T>(string name)
		{
			var option = new OptionWithArgument<T>
			{
				Name = currentOption.Name,
				ShortName = currentOption.ShortName.HasValue ? currentOption.ShortName.Value.ToString() : null,
				Description = currentOption.Description,
				ArgumentName = name
			};
			Options.Add(option);
			return option;
		}

		public IArg Required { get { currentOption = new OptionSpec { Required = true }; return this; } }

		public IArg<T> Arg<T>(string name, string description)
		{
			var option = new Argument<T>
			{
				Name = name,
				Description = description,
				Required = currentOption != null ? currentOption.Required : false
			};
			Options.Add(option);
			currentOption = null;
			return option;
		}

		public List<OptionBase> Options = new List<OptionBase>();

		private OptionSpec currentOption;

		private class OptionSpec
		{
			public string Name;
			public char? ShortName;
			public string Description;
			public bool Required;
		}
	}

}
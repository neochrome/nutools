using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuTools.Common
{
	public class Inputs
	{
		public Inputs(IEnumerable<string> filesOrStandardInput)
		{
			var fileNames = new Glob(Environment.CurrentDirectory);

			filesOrStandardInput.Except(Input.IsStandardInput).Each(fileNames.Include);
			inputs = fileNames.Select(f => new Input(f)).ToList();
			if (filesOrStandardInput.Any(Input.IsStandardInput) || inputs.Count == 0)
				inputs.Insert(0, Input.FromStandardInput());
		}

		public bool Any { get { return inputs.Count > 1; } }

		public void Process(Action<string, StreamReader> process)
		{
			Process((name, reader) => { process(name, reader); return true; });
		}
		
		public void Process(Func<string, StreamReader, bool> process)
		{
			foreach (var input in inputs)
			{
				using (var reader = new StreamReader(input.Open()))
				{
					if (!process(input.Name, reader)) break;
				}
			}
		}

		private readonly List<Input> inputs;
	}
}
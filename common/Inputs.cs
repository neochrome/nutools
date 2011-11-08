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
			Func<string, bool> fromStdIn = f => f == "-";
			var fileNames = new Glob(Environment.CurrentDirectory);

			filesOrStandardInput.Except(fromStdIn).Each(fileNames.Include);
			inputs = fileNames.Select(f => new Input { Name = f, Open = () => File.OpenText(f) }).ToList();
			if (filesOrStandardInput.Any(fromStdIn) || inputs.Count == 0)
				inputs.Insert(0, new Input { Name = "(standard input)", Open = () => new StreamReader(Console.OpenStandardInput()) });
		}

		public bool Any { get { return inputs.Count > 1; } }

		public void Process(Func<string, StreamReader, bool> process)
		{
			foreach (var input in inputs)
			{
				using (var reader = input.Open())
				{
					if (!process(input.Name, reader)) break;
				}
			}
		}

		private struct Input
		{
			public string Name;
			public Func<StreamReader> Open;
		}
		private readonly List<Input> inputs;
	}
}
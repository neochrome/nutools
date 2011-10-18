using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace NuTools.Common
{
	public class Glob : List<string>
	{
		public Glob() : this(Environment.CurrentDirectory) { }
		public Glob(string path) : this(path, new DirectoryWrapper()) { }

		public Glob(string path, IDirectory directory)
		{
			this.path = path;
			this.directory = directory;
		}

		public void Include(string pattern)
		{
			IncludeRecursive(path, pattern.Split(new[] { '/', '\\' }));
		}

		public void Exclude(string pattern)
		{
			var translate = new Dictionary<string, string> { { "*", @".*" } };
			var expression = new Regex(@"^" + Regex.Replace(pattern, @"\*", match => translate[match.Value]) + @"$");
			RemoveAll(f => expression.IsMatch(f));
		}

		private void IncludeRecursive(string path, string[] segments)
		{
			var pattern = segments.First();
			var otherSegments = segments.Skip(1).ToArray();
			if (otherSegments.Any())
			{
				directory
					.EnumerateDirectories(path, pattern, pattern == "**" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
					.Each(sub => IncludeRecursive(sub, otherSegments));
			}
			else
			{
				var files = directory.EnumerateFiles(path, pattern)
					.Select(f => f.Replace(path, string.Empty).TrimStart('/', '\\'));
				AddRange(files);
			}
		}

		private readonly IDirectory directory;
		private readonly string path;
	}

	public interface IDirectory
	{
		IEnumerable<string> EnumerateFiles(string path, string pattern);
		IEnumerable<string> EnumerateDirectories(string path, string pattern, SearchOption searchOption);
	}

	public class DirectoryWrapper : IDirectory
	{
		public IEnumerable<string> EnumerateFiles(string path, string pattern)
		{
			return Directory.GetFiles(path, pattern);
		}

		public IEnumerable<string> EnumerateDirectories(string path, string pattern, SearchOption searchOption)
		{
			return Directory.GetDirectories(path, pattern, searchOption);
		}
	}
}

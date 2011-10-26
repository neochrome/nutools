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

		public Glob(string path)
		{
			this.path = path;
		}

		public void Include(string pattern)
		{
			IncludeRecursive(path, pattern.Split(pathDelimiters));
		}

		public void Exclude(string pattern)
		{
			var expression = new Regex(@"^" + Regex.Replace(pattern, @"\*", match => mapWildCardToRegex[match.Value]) + @"$");
			RemoveAll(f => expression.IsMatch(f));
		}

		private void IncludeRecursive(string path, string[] segments)
		{
			var pattern = segments.First();
			var otherSegments = segments.Skip(1).ToArray();
			if (otherSegments.Any())
			{
				Directory
					.GetDirectories(path, pattern, pattern == "**" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
					.Each(sub => IncludeRecursive(sub, otherSegments));
			}
			else
			{
				var files = 
					Directory.GetFiles(path, pattern)
					.Select(f => f.Replace(path, string.Empty).TrimStart('/', '\\'));
				AddRange(files);
			}
		}

		private readonly string path;
		private static readonly char[] pathDelimiters = new [] { '/', '\\' };
		private static readonly Dictionary<string, string> mapWildCardToRegex = new Dictionary<string, string> { { "*", @".*" } };

	}
}

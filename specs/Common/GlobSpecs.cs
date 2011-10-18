using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Cone;

using NuTools.Common;

namespace NuTools.Specs.Common
{
	[Describe(typeof(Glob))]
	public class GlobSpecs
	{
		public void should_be_empty_when_created()
		{
			Verify.That(() => new Glob().Count == 0);
		}

		public void should_include_all_files_from_path()
		{
			var allFiles = new[] { "file1.ext", "file2.ext", "file3.ext" };
			var directory = A.Fake<IDirectory>();
			A.CallTo(directory).WithReturnType<IEnumerable<string>>().Returns(allFiles);
			var target = new Glob(path, directory);

			target.Include("*");

			Verify.That(() => target.Count == allFiles.Length);
		}

		public void should_strip_path_from_included_file()
		{
			var directory = A.Fake<IDirectory>();
			A.CallTo(directory).WithReturnType<IEnumerable<string>>().Returns(new[] { Path.Combine(path, "file.ext") });
			var target = new Glob(path, directory);

			target.Include("*");

			Verify.That(() => target[0] == "file.ext");
		}

		public void should_include_files_from_path_matching_the_given_pattern()
		{
			var directory = A.Fake<IDirectory>();
			var target = new Glob(path, directory);

			target.Include("*.ext");

			A.CallTo(() => directory.EnumerateFiles(path, "*.ext")).MustHaveHappened();
		}

		public void should_include_files_from_sub_path_in_the_given_pattern()
		{
			var subPath = Path.Combine(path, "sub");
			var directory = A.Fake<IDirectory>();
			A.CallTo(() => directory.EnumerateDirectories(null, null, SearchOption.AllDirectories))
				.WithAnyArguments().Returns(new[] { subPath });
			var target = new Glob(path, directory);

			target.Include("sub/*.ext");

			A.CallTo(() => directory.EnumerateDirectories(path, "sub", SearchOption.TopDirectoryOnly)).MustHaveHappened();
			A.CallTo(() => directory.EnumerateFiles(subPath, "*.ext")).MustHaveHappened();
		}

		public void should_include_files_from_sub_path_at_any_depth()
		{
			var subPath = Path.Combine(Path.Combine(path, "any"), "sub");
			var directory = A.Fake<IDirectory>();
			A.CallTo(() => directory.EnumerateDirectories(null, null, SearchOption.AllDirectories))
				.WithAnyArguments().Returns(new[] { subPath });
			var target = new Glob(path, directory);

			target.Include("**/sub/*.txt");

			A.CallTo(() => directory.EnumerateDirectories(path, "**", SearchOption.AllDirectories)).MustHaveHappened();
			A.CallTo(() => directory.EnumerateFiles(subPath, "*.txt")).MustHaveHappened();
		}

		public void should_exclude_file_matching_pattern()
		{
			var target = new Glob { "file1.ext", "file2.ext2" };

			target.Exclude("file1.ext");

			Verify.That(() => target.SequenceEqual(new[] { "file2.ext2" }));
		}

		public void should_exclude_all_files_matching_pattern()
		{
			var target = new Glob { "file1.ext", "file2.ext", "file3.doc" };

			target.Exclude("*.ext");

			Verify.That(() => target.SequenceEqual(new[] { "file3.doc" }));
		}

		const string path = "path";
	}
}
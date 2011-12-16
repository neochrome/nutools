using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NuTools.Common;

namespace NuTools
{
	static class App
	{
		public static IEnumerable<Type> Commands
		{
			get
			{
				return Assembly.GetExecutingAssembly()
					.GetTypes()
					.Where(typeof(ICommand).IsAssignableFrom)
					.Where(t => t.IsVisible && t.IsClass);
			}
		}

		public static IEnumerable<Type> Except<T>(this IEnumerable<Type> source)
		{
			return source.Except(t => t == typeof(T));
		}

		public static string CommandName
		{
			get { return Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName).ToLower(); }
		}
	}
}
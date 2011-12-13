using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NuTools
{
	static class All
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
	}
}
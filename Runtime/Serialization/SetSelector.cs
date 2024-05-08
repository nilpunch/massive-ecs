using System;
using System.Collections.Generic;

namespace Massive.Serialization
{
	/// <summary>
	/// Helper delegate to select sets from any registry.
	/// </summary>
	public delegate IReadOnlyList<IReadOnlySet> SetSelector(IRegistry registry);

	public static class Select
	{
		public static IReadOnlyList<IReadOnlySet> Nothing(IRegistry registry)
		{
			return Array.Empty<IReadOnlySet>();
		}

		public static IReadOnlyList<IReadOnlySet> Many<T>(IRegistry registry)
		{
			return registry.Many<T>();
		}

		public static IReadOnlyList<IReadOnlySet> Many<T1, T2>(IRegistry registry)
		{
			return registry.Many<T1, T2>();
		}

		public static IReadOnlyList<IReadOnlySet> Many<T1, T2, T3>(IRegistry registry)
		{
			return registry.Many<T1, T2, T3>();
		}
	}
}

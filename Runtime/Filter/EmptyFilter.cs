using System;
using System.Collections.Generic;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		public IReadOnlyList<IReadOnlySet> Include => Array.Empty<IReadOnlySet>();
		public IReadOnlyList<IReadOnlySet> Exclude => Array.Empty<IReadOnlySet>();

		private EmptyFilter()
		{
		}

		public bool ContainsId(int id)
		{
			return true;
		}
	}
}

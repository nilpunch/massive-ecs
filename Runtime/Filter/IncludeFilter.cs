using System;
using System.Collections.Generic;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		public IReadOnlyList<IReadOnlySet> Include { get; }
		public IReadOnlyList<IReadOnlySet> Exclude => Array.Empty<IReadOnlySet>();

		public IncludeFilter(IReadOnlyList<IReadOnlySet> include = null)
		{
			Include = include ?? Array.Empty<IReadOnlySet>();
		}

		public bool ContainsId(int id)
		{
			for (int i = 0; i < Include.Count; i++)
			{
				if (!Include[i].IsAssigned(id))
				{
					return false;
				}
			}

			return true;
		}
	}
}

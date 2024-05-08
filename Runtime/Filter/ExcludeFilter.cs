using System;
using System.Collections.Generic;

namespace Massive
{
	public class ExcludeFilter : IFilter
	{
		public IReadOnlyList<IReadOnlySet> Include => Array.Empty<IReadOnlySet>();
		public IReadOnlyList<IReadOnlySet> Exclude { get; }

		public ExcludeFilter(IReadOnlyList<IReadOnlySet> exclude = null)
		{
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();
		}

		public bool ContainsId(int id)
		{
			for (int i = 0; i < Exclude.Count; i++)
			{
				if (Exclude[i].IsAssigned(id))
				{
					return false;
				}
			}

			return true;
		}
	}
}

using System;
using System.Collections.Generic;

namespace Massive
{
	public class Filter : IFilter
	{
		public IReadOnlyList<IReadOnlySet> Include { get; }
		public IReadOnlyList<IReadOnlySet> Exclude { get; }

		public Filter(IReadOnlyList<IReadOnlySet> include = null, IReadOnlyList<IReadOnlySet> exclude = null)
		{
			Include = include ?? Array.Empty<IReadOnlySet>();
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();

			for (int i = 0; i < Exclude.Count; i++)
			{
				if (Include.Contains(Exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
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

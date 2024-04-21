using System;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		public IReadOnlySet[] Include { get; }
		public IReadOnlySet[] Exclude => Array.Empty<IReadOnlySet>();

		public IncludeFilter(IReadOnlySet[] include = null)
		{
			Include = include ?? Array.Empty<IReadOnlySet>();
		}

		public bool ContainsId(int id)
		{
			for (int i = 0; i < Include.Length; i++)
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
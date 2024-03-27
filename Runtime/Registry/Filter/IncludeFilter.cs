using System;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		public IReadOnlySet[] Include { get; }
		public IReadOnlySet[] Exclude => Array.Empty<ISet>();

		public IncludeFilter(ISet[] include = null)
		{
			Include = include ?? Array.Empty<ISet>();
		}

		public bool Contains(int id)
		{
			for (int i = 0; i < Include.Length; i++)
			{
				if (!Include[i].IsAlive(id))
				{
					return false;
				}
			}

			return true;
		}
	}
}
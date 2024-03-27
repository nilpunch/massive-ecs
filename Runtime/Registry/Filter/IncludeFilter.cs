using System;
using System.Linq;

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

		public bool IsSubsetOf(IFilter other)
		{
			for (int i = 0; i < Include.Length; i++)
			{
				if (!other.Include.Contains(Include[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
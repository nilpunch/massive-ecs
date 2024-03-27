using System;

namespace Massive
{
	public class ExcludeFilter : IFilter
	{
		public IReadOnlySet[] Include => Array.Empty<IReadOnlySet>();
		public IReadOnlySet[] Exclude { get; }

		public ExcludeFilter(IReadOnlySet[] exclude = null)
		{
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();
		}

		public bool Contains(int id)
		{
			for (int i = 0; i < Exclude.Length; i++)
			{
				if (Exclude[i].IsAlive(id))
				{
					return false;
				}
			}

			return true;
		}

		public bool IsSubsetOf(IFilter other)
		{
			return Exclude.IsSubsetOf(other.Exclude);
		}
	}
}
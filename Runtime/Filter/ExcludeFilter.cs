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

		public bool ContainsId(int id)
		{
			for (int i = 0; i < Exclude.Length; i++)
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

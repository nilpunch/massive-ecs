using System;

namespace Massive
{
	public class ExcludeFilter : IFilter
	{
		public ISet[] Exclude { get; }

		public ExcludeFilter(ISet[] exclude = null)
		{
			Exclude = exclude ?? Array.Empty<ISet>();
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
	}
}
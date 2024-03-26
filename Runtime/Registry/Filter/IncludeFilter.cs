using System;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		public ISet[] Include { get; }
		public ISet[] Exclude => Array.Empty<ISet>();

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
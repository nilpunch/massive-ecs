using System;

namespace Massive
{
	public class Filter : IFilter
	{
		public ISet[] Include { get; }
		public ISet[] Exclude { get; }

		public Filter(ISet[] include = null, ISet[] exclude = null)
		{
			Include = include ?? Array.Empty<ISet>();
			Exclude = exclude ?? Array.Empty<ISet>();
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
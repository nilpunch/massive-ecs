using System;

namespace Massive
{
	public class Filter : IFilter
	{
		public IReadOnlySet[] Include { get; }
		public IReadOnlySet[] Exclude { get; }

		public Filter(IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			Include = include ?? Array.Empty<IReadOnlySet>();
			Exclude = exclude ?? Array.Empty<IReadOnlySet>();

			for (int i = 0; i < Exclude.Length; i++)
			{
				if (Include.Contains(Exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
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

		public bool IsSubsetOf(IFilter other)
		{
			for (int i = 0; i < Include.Length; i++)
			{
				if (!other.Include.Contains(Include[i]))
				{
					return false;
				}
			}

			for (int i = 0; i < Exclude.Length; i++)
			{
				if (!other.Exclude.Contains(Exclude[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
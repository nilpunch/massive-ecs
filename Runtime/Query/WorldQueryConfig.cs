using System.Linq;

namespace Massive
{
	public struct WorldQueryConfig
	{
		private readonly BitSet[] ExcludedImplicitly;
		private readonly int ExcludedImplicitlyLength;

		public WorldQueryConfig(World world)
		{
			ExcludedImplicitly = world.Config.ExcludedImplicitly
				.SelectMany(setSelector => setSelector.Select(world.Sets))
				.Distinct()
				.ToArray();
			ExcludedImplicitlyLength = ExcludedImplicitly.Length;
		}

		public void ApplyImplicitExclusion(Filter filter, QueryCache resultQueryCache)
		{
			for (var i = 0; i < ExcludedImplicitlyLength; i++)
			{
				var implicitSet = ExcludedImplicitly[i];

				var isIncludedExplicitly = false;
				for (var j = 0; j < filter.IncludeCount; j++)
				{
					if (implicitSet == filter.Included[j])
					{
						isIncludedExplicitly = true;
						break;
					}
				}

				if (!isIncludedExplicitly)
				{
					resultQueryCache.AddExclude(implicitSet);
				}
			}
		}
	}
}

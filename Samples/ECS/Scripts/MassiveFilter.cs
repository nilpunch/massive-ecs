using System;

namespace Massive.Samples.ECS
{
	public class MassiveFilter
	{
		public static MassiveFilter Default { get; } = new MassiveFilter();

		private readonly ArraySegment<IReadOnlySet> _mustInclude;
		private readonly ArraySegment<IReadOnlySet> _mustExclude;
		
		public MassiveFilter(IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			_mustInclude = include ?? ArraySegment<IReadOnlySet>.Empty;
			_mustExclude = exclude ?? ArraySegment<IReadOnlySet>.Empty;
		}

		public bool CheckEntity(int entity)
		{
			foreach (var exclude in _mustExclude)
			{
				if (exclude.IsAlive(entity))
				{
					return false;
				}
			}
			
			foreach (var include in _mustInclude)
			{
				if (!include.IsAlive(entity))
				{
					return false;
				}
			}

			return true;
		}
	}
}
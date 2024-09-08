using System;

namespace Massive
{
	public class Filter : IFilter
	{
		private readonly ArraySegment<SparseSet> _exclude;
		private readonly ArraySegment<SparseSet> _include;

		public Filter(ArraySegment<SparseSet> include, ArraySegment<SparseSet> exclude)
		{
			_include = include;
			_exclude = exclude;

			for (int i = 0; i < _exclude.Count; i++)
			{
				if (_include.Contains(_exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
		}

		public ArraySegment<SparseSet> Include => _include;

		public bool ContainsId(int id)
		{
			return id >= 0 && SetHelpers.AssignedInAll(id, _include) && SetHelpers.NotAssignedInAll(id, _exclude);
		}
	}
}

using System;

namespace Massive
{
	public class Filter : IFilter
	{
		private readonly SparseSet[] _exclude;
		private readonly SparseSet[] _include;

		public Filter(SparseSet[] include, SparseSet[] exclude)
		{
			_include = include;
			_exclude = exclude;

			for (int i = 0; i < _exclude.Length; i++)
			{
				if (_include.Contains(_exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
		}

		public SparseSet[] Include => _include;

		public bool ContainsId(int id)
		{
			return id >= 0 && SetHelpers.AssignedInAll(id, _include) && SetHelpers.NotAssignedInAll(id, _exclude);
		}
	}
}

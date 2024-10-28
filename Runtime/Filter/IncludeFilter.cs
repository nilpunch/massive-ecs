using System;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		private readonly SparseSet[] _include;

		public IncludeFilter(SparseSet[] include)
		{
			_include = include;
		}

		public SparseSet[] Include => _include;

		public bool ContainsId(int id)
		{
			return id >= 0 && SetHelpers.AssignedInAll(id, _include);
		}
	}
}

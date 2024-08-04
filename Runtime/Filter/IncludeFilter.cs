using System;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		private readonly ArraySegment<SparseSet> _include;

		public IncludeFilter(ArraySegment<SparseSet> include)
		{
			_include = include;
		}

		public ArraySegment<SparseSet> Include => _include;

		public bool ContainsId(int id)
		{
			return SetHelpers.AssignedInAll(id, _include);
		}
	}
}

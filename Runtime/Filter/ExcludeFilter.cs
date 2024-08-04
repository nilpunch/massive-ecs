using System;

namespace Massive
{
	public class ExcludeFilter : IFilter
	{
		private readonly ArraySegment<SparseSet> _exclude;

		public ExcludeFilter(ArraySegment<SparseSet> exclude)
		{
			_exclude = exclude;
		}

		public ArraySegment<SparseSet> Include => ArraySegment<SparseSet>.Empty;

		public bool ContainsId(int id)
		{
			return SetHelpers.NotAssignedInAll(id, _exclude);
		}
	}
}

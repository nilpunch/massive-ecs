using System;

namespace Massive
{
	public class ExcludeFilter : IFilter
	{
		private readonly ArraySegment<IReadOnlySet> _exclude;

		public ExcludeFilter(ArraySegment<IReadOnlySet> exclude)
		{
			_exclude = exclude;
		}

		public ArraySegment<IReadOnlySet> Include => ArraySegment<IReadOnlySet>.Empty;

		public bool ContainsId(int id)
		{
			return SetHelpers.NotAssignedInAll(id, _exclude);
		}
	}
}

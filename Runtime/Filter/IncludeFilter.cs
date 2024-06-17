using System;

namespace Massive
{
	public class IncludeFilter : IFilter
	{
		private readonly ArraySegment<IReadOnlySet> _include;

		public IncludeFilter(ArraySegment<IReadOnlySet> include)
		{
			_include = include;
		}

		public ArraySegment<IReadOnlySet> Include => _include;

		public bool ContainsId(int id)
		{
			return SetHelpers.AssignedInAll(id, _include);
		}
	}
}

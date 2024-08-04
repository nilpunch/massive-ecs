using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		private EmptyFilter()
		{
		}

		public ArraySegment<SparseSet> Include => ArraySegment<SparseSet>.Empty;

		public bool ContainsId(int id)
		{
			return true;
		}
	}
}

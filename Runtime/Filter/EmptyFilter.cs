using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		private EmptyFilter()
		{
		}

		public ArraySegment<IReadOnlySet> Include => ArraySegment<IReadOnlySet>.Empty;

		public bool ContainsId(int id)
		{
			return true;
		}
	}
}

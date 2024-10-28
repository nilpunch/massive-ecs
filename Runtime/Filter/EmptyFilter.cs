using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		private EmptyFilter()
		{
		}

		public SparseSet[] Include => Array.Empty<SparseSet>();

		public bool ContainsId(int id)
		{
			return id >= 0;
		}
	}
}

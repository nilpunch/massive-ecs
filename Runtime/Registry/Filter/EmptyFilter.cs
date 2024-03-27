using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		public IReadOnlySet[] Include => Array.Empty<ISet>();
		public IReadOnlySet[] Exclude => Array.Empty<ISet>();

		private EmptyFilter()
		{
		}

		public bool Contains(int id)
		{
			return true;
		}
	}
}
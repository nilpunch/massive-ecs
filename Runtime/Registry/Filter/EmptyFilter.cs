using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		public IReadOnlySet[] Include => Array.Empty<IReadOnlySet>();
		public IReadOnlySet[] Exclude => Array.Empty<IReadOnlySet>();

		private EmptyFilter() { }

		public bool ContainsId(int id)
		{
			return true;
		}
	}
}
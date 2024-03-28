using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		public IReadOnlySet[] Include => Array.Empty<IReadOnlySet>();
		public IReadOnlySet[] Exclude => Array.Empty<IReadOnlySet>();

		private EmptyFilter()
		{
		}

		public bool ContainsId(int id)
		{
			return true;
		}

		public bool Contains(IFilter other)
		{
			return other.Include.Length == 0 && other.Exclude.Length == 0;
		}
	}
}
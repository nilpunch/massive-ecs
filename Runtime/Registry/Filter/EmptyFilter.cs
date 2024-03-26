using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public static EmptyFilter Instance { get; } = new EmptyFilter();

		public ISet[] Include => Array.Empty<ISet>();
		public ISet[] Exclude => Array.Empty<ISet>();

		private EmptyFilter()
		{
		}

		public bool Contains(int id)
		{
			return true;
		}
	}
}
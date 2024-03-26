using System;

namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public ISet[] Include => Array.Empty<ISet>();
		public ISet[] Exclude => Array.Empty<ISet>();

		public bool Contains(int id)
		{
			return true;
		}
	}
}
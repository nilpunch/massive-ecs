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
			for (int i = 0; i < _include.Count; i++)
			{
				if (!_include[i].IsAssigned(id))
				{
					return false;
				}
			}

			return true;
		}
	}
}

using System;

namespace Massive
{
	public class Filter : IFilter
	{
		private readonly ArraySegment<IReadOnlySet> _exclude;
		private readonly ArraySegment<IReadOnlySet> _include;

		public Filter(ArraySegment<IReadOnlySet> include, ArraySegment<IReadOnlySet> exclude)
		{
			_include = include;
			_exclude = exclude;

			for (int i = 0; i < _exclude.Count; i++)
			{
				if (_include.Contains(_exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
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

			for (int i = 0; i < _exclude.Count; i++)
			{
				if (_exclude[i].IsAssigned(id))
				{
					return false;
				}
			}

			return true;
		}
	}
}

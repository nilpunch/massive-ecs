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
			int includeOr = 0;
			for (int i = 0; i < _include.Count; i++)
			{
				includeOr |= _include[i].GetDenseOrInvalid(id);
			}

			return includeOr != Constants.InvalidId;
		}
	}
}

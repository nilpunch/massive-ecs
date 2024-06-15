using System;

namespace Massive
{
	public class ExcludeFilter : IFilter
	{
		private readonly ArraySegment<IReadOnlySet> _exclude;

		public ExcludeFilter(ArraySegment<IReadOnlySet> exclude)
		{
			_exclude = exclude;
		}

		public ArraySegment<IReadOnlySet> Include => ArraySegment<IReadOnlySet>.Empty;

		public bool ContainsId(int id)
		{
			int excludeAnd = Constants.InvalidId;
			for (int i = 0; i < _exclude.Count; i++)
			{
				excludeAnd &= _exclude[i].GetDenseOrInvalid(id);
			}

			return excludeAnd == Constants.InvalidId;
		}
	}
}

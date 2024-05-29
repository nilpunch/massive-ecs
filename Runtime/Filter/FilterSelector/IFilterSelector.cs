using System;

namespace Massive
{
	public interface IFilterSelector
	{
		public int IncludeCount { get; }
		public int ExcludeCount { get; }

		void Select(ArraySegment<IReadOnlySet> include, ArraySegment<IReadOnlySet> exclude);
	}
}

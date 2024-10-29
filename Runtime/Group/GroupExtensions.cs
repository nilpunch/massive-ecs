using System;

namespace Massive
{
	public static class GroupExtensions
	{
		public static ReadOnlySpan<int> AsIds(this IGroup group)
		{
			return new ReadOnlySpan<int>(group.MainSet.Ids, 0, group.Count);
		}
	}
}

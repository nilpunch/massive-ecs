using System;

namespace Massive
{
	public interface IFilter
	{
		ArraySegment<SparseSet> Include { get; }

		bool ContainsId(int id);
	}
}

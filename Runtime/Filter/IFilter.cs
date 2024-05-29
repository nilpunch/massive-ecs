using System;

namespace Massive
{
	public interface IFilter
	{
		ArraySegment<IReadOnlySet> Include { get; }

		bool ContainsId(int id);
	}
}

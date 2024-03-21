using System;

namespace Massive
{
	public interface IReadOnlySet
	{
		int AliveCount { get; }

		ReadOnlySpan<int> AliveIds { get; }

		bool TryGetDense(int id, out int dense);

		bool IsAlive(int id);
	}
}
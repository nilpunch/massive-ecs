using System;

namespace Massive
{
	public interface IReadOnlySet
	{
		int AliveCount { get; }

		ReadOnlySpan<int> AliveIds { get; }

		event Action<int> Added;

		event Action<(int Id, int Dense)> Removed;

		int GetDense(int id);

		bool TryGetDense(int id, out int dense);

		bool IsAlive(int id);
	}
}
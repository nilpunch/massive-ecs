using System;
using System.Diagnostics.Contracts;

namespace Massive
{
	public interface IReadOnlySet
	{
		[Pure]
		int Capacity { get; }

		[Pure]
		int AliveCount { get; }

		[Pure]
		ReadOnlySpan<int> AliveIds { get; }

		[Pure]
		int GetDense(int id);

		[Pure]
		bool TryGetDense(int id, out int dense);

		[Pure]
		bool IsAlive(int id);
	}
}
using System;

namespace Massive
{
	public interface IReadOnlySet
	{
		int AliveCount { get; }
		
		ReadOnlySpan<int> AliveIds { get; }
		
		int GetDense(int id);
		
		bool TryGetDense(int id, out int dense);
		
		bool IsAlive(int id);
	}
}
using System;

namespace Massive
{
	public interface IReadOnlySet
	{
		int AliveCount { get; }

		ReadOnlySpan<int> AliveIds { get; }

		/// <summary>
		/// Shoots only after <see cref="Massive.ISet.Ensure(int)"/> call, when the id was not alive and therefore was created.
		/// </summary>
		event Action<int> AfterAdded;

		/// <summary>
		/// Shoots before each <see cref="Massive.ISet.Remove(int)"/> call, even if the id is already dead.
		/// </summary>
		event Action<int> BeforeRemoved;

		int GetDense(int id);

		bool TryGetDense(int id, out int dense);

		bool IsAlive(int id);
	}
}
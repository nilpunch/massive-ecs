using System;

namespace Massive
{
	public interface IReadOnlySet
	{
		int Count { get; }

		ReadOnlyPackedSpan<int> Ids { get; }

		/// <summary>
		/// Shoots only after <see cref="ISet.Assign"/> call, when the id was not alive and therefore was created.
		/// </summary>
		event Action<int> AfterAssigned;

		/// <summary>
		/// Shoots before each <see cref="ISet.Unassign"/> call, even if the id is already dead.
		/// </summary>
		event Action<int> BeforeUnassigned;

		int GetDense(int id);

		bool TryGetDense(int id, out int dense);

		bool IsAssigned(int id);
	}
}

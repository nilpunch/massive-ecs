using System;

namespace Massive
{
	public interface IReadOnlySet
	{
		/// <summary>
		/// Shoots only after <see cref="ISet.Assign"/> call, when the id was not alive and therefore was created.
		/// </summary>
		event Action<int> AfterAssigned;

		/// <summary>
		/// Shoots before each <see cref="ISet.Unassign"/> call, when the id was alive.
		/// </summary>
		event Action<int> BeforeUnassigned;

		bool IsAssigned(int id);
	}
}

using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class ManagedDataSet<T> : DataSet<T>
	{
		public ManagedDataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(pageSize, packing)
		{
		}

		/// <summary>
		/// Moves the data from one index to another.
		/// </summary>
		protected override void MoveDataAt(int source, int destination)
		{
			Data.Swap(source, destination);
		}
	}
}

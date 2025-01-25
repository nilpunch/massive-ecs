using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// Swaps data when elements are moved.
	/// Used in registry for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SwappingDataSet<T> : DataSet<T>
	{
		public SwappingDataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(pageSize, packing)
		{
		}

		protected override void MoveDataAt(int source, int destination)
		{
			Data.Swap(source, destination);
		}
	}
}

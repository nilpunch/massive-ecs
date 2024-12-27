using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.DataSet{T}"/>.
	/// Swaps data when elements are moved.
	/// Used in registry for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveSwappingDataSet<T> : MassiveDataSet<T>
	{
		public MassiveSwappingDataSet(int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(framesCapacity, pageSize, packing)
		{
		}

		public override void MoveDataAt(int source, int destination)
		{
			Data.Swap(source, destination);
		}
	}
}

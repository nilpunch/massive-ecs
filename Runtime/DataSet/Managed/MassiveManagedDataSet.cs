using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.ManagedDataSet{T}"/>.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveManagedDataSet<T> : MassiveDataSet<T>
	{
		public MassiveManagedDataSet(int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(framesCapacity, pageSize, packing)
		{
		}

		protected override void MoveDataAt(int source, int destination)
		{
			Data.Swap(source, destination);
		}
	}
}

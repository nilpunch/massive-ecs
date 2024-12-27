using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/> with custom copying.
	/// Swaps data when elements are moved.
	/// Used in registry for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class CopyingDataSet<T> : SwappingDataSet<T> where T : ICopyable<T>
	{
		public CopyingDataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(pageSize, packing)
		{
		}

		public override void CopyDataAt(int source, int destination)
		{
			Data[source].CopyTo(ref Data[destination]);
		}
	}
}

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class DataSet<T> : SparseSet, IDataSet
	{
		public PagedArray<T> Data { get; }

		public DataSet(int pageSize = Constants.DefaultPageSize, PackingMode packingMode = PackingMode.Continuous)
			: base(packingMode)
		{
			Data = new PagedArray<T>(pageSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyData(int sourceId, int targetId)
		{
			Get(targetId) = Get(sourceId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Data[Sparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SwapPacked(int first, int second)
		{
			base.SwapPacked(first, second);
			Data.Swap(first, second);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyFromToPacked(int source, int destination)
		{
			base.CopyFromToPacked(source, destination);
			Data[destination] = Data[source];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void EnsureDataForIndex(int index)
		{
			Data.EnsurePageForIndex(index);
		}

		IPagedArray IDataSet.Data => Data;

		object IDataSet.GetRaw(int id) => Get(id);

		void IDataSet.SetRaw(int id, object value) => Get(id) = (T)value;
	}
}

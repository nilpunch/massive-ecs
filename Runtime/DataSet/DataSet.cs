using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for any <see cref="Massive.ISet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : SparseSet, IDataSet<T> where T : struct
	{
		protected T[] Data { get; }

		public DataSet(int dataCapacity = Constants.DataCapacity)
			: base(dataCapacity)
		{
			Data = new T[Dense.Length];
		}

		public Span<T> AliveData => new Span<T>(Data, 0, AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Ensure(int id, T data)
		{
			base.Ensure(id);
			Data[Sparse[id]] = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Data[Sparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SwapDense(int denseA, int denseB)
		{
			base.SwapDense(denseA, denseB);
			(Data[denseA], Data[denseB]) = (Data[denseB], Data[denseA]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyFromToDense(int source, int destination)
		{
			base.CopyFromToDense(source, destination);
			Data[destination] = Data[source];
		}
	}
}
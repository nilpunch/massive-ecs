using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveDataSet<T> : DataSet<T>, IMassive where T : struct
	{
		private readonly SparseSetFrames _sparseSetFrames;
		private readonly T[][] _dataByFrames;

		public MassiveDataSet(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_sparseSetFrames = new SparseSetFrames(DenseCapacity, SparseCapacity, framesCapacity);

			_dataByFrames = new T[framesCapacity][];

			for (int i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new T[DenseCapacity];
			}
		}

		public int CanRollbackFrames => _sparseSetFrames.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_sparseSetFrames.SaveFrame(this);

			// We can sync saving with MassiveSparseSet saving, using its CurrentFrame
			Array.Copy(RawData, 0, _dataByFrames[_sparseSetFrames.CurrentFrame], 0, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_sparseSetFrames.Rollback(frames, this);

			// Similarly to saving, we can sync rollback with MassiveSparseSet rollback
			Array.Copy(_dataByFrames[_sparseSetFrames.CurrentFrame], 0, RawData, 0, Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeDense(int capacity)
		{
			base.ResizeDense(capacity);

			_sparseSetFrames.ResizeDense(capacity);

			for (int i = 0; i < _dataByFrames.Length; i++)
			{
				Array.Resize(ref _dataByFrames[i], capacity);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeSparse(int capacity)
		{
			base.ResizeSparse(capacity);
			_sparseSetFrames.ResizeSparse(capacity);
		}
	}
}
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
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_sparseSetFrames = new SparseSetFrames(this, framesCapacity);

			_dataByFrames = new T[framesCapacity * Data.Length];
		}

		public int CanRollbackFrames => _sparseSetFrames.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_sparseSetFrames.SaveFrame();

			// We can sync saving with MassiveSparseSet saving, using its CurrentFrame
			Array.Copy(Data, 0, _dataByFrames, _sparseSetFrames.CurrentFrame * Data.Length, AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_sparseSetFrames.Rollback(frames);

			// Similarly to saving, we can sync rollback with MassiveSparseSet rollback
			Array.Copy(_dataByFrames, _sparseSetFrames.CurrentFrame * Data.Length, Data, 0, AliveCount);
		}
	}
}
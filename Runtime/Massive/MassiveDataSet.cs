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
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveDataSet<T> : DataSetBase<T, MassiveSparseSet>, IMassiveSet where T : unmanaged
	{
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSparseSet(framesCapacity, dataCapacity))
		{
			_dataByFrames = new T[framesCapacity * Data.Length];
		}

		public int CanRollbackFrames => SparseSet.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			SparseSet.SaveFrame();

			// We can sync saving with MassiveSparseSet saving, using its CurrentFrame
			Array.Copy(Data, 0, _dataByFrames, SparseSet.CurrentFrame * Data.Length, AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			SparseSet.Rollback(frames);

			// Similarly to saving, we can sync rollback with MassiveSparseSet rollback
			Array.Copy(_dataByFrames, SparseSet.CurrentFrame * Data.Length, Data, 0, AliveCount);
		}
	}
}
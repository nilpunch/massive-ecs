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
	public class MassiveDataSet<T> : DataSet<T>, IMassiveSet where T : unmanaged
	{
		private readonly MassiveSparseSet _sparseSet;
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSparseSet(framesCapacity, dataCapacity))
		{
			// Fetch instance from base
			_sparseSet = (MassiveSparseSet)SparseSet;
			_dataByFrames = new T[framesCapacity * Data.Length];
		}

		public int CanRollbackFrames => _sparseSet.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_sparseSet.SaveFrame();

			// We can sync saving with MassiveSparseSet saving, using its CurrentFrame
			Array.Copy(Data, 0, _dataByFrames, _sparseSet.CurrentFrame * Data.Length, AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_sparseSet.Rollback(frames);

			// Similarly to saving, we can sync rollback with MassiveSparseSet rollback
			Array.Copy(_dataByFrames, _sparseSet.CurrentFrame * Data.Length, Data, 0, AliveCount);
		}
	}
}
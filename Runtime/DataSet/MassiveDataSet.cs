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
	public class MassiveDataSet<T> : DataSet<T>, IMassive where T : struct
	{
		private readonly MassiveSparseSet _massiveSparseSet;
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(new MassiveSparseSet(dataCapacity, framesCapacity))
		{
			// Fetch instance from base
			_massiveSparseSet = (MassiveSparseSet)SparseSet;

			_dataByFrames = new T[framesCapacity * Data.Length];
		}

		public int CanRollbackFrames => _massiveSparseSet.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveSparseSet.SaveFrame();

			// We can sync saving with MassiveSparseSet saving, using its CurrentFrame
			Array.Copy(Data, 0, _dataByFrames, _massiveSparseSet.CurrentFrame * Data.Length, AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveSparseSet.Rollback(frames);

			// Similarly to saving, we can sync rollback with MassiveSparseSet rollback
			Array.Copy(_dataByFrames, _massiveSparseSet.CurrentFrame * Data.Length, Data, 0, AliveCount);
		}
	}
}
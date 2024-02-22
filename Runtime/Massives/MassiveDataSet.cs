using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/>.
	/// </summary>
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class MassiveDataSet<T> : DataSetBase<T, MassiveSparseSet>, IMassive where T : struct
	{
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSparseSet(dataCapacity))
		{
			_dataByFrames = new T[framesCapacity * Data.Length];
		}

		public int CurrentFrame => SparseSet.CurrentFrame;

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
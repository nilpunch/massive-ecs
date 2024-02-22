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
	public class MassiveDataSet<T> : DataSet<T>, IMassive where T : struct
	{
		private readonly MassiveSparseSet _massiveSparseSet;
		private readonly T[] _dataByFrames;

		public MassiveDataSet(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSparseSet(dataCapacity))
		{
			_massiveSparseSet = (MassiveSparseSet)SparseSet;
			_dataByFrames = new T[framesCapacity * Data.Length];
		}

		public int CanRollbackFrames => _massiveSparseSet.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveSparseSet.SaveFrame();
			Array.Copy(Data, 0, _dataByFrames, _massiveSparseSet.CurrentFrame * Data.Length, AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveSparseSet.Rollback(frames);
			Array.Copy(_dataByFrames, _massiveSparseSet.CurrentFrame * Data.Length, Data, 0, AliveCount);
		}
	}
}
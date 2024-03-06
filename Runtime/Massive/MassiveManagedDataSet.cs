using System.Runtime.CompilerServices;
using Massive.ECS;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/> with managed data support.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveManagedDataSet<T> : ManagedDataSet<T>, IMassive where T : struct
	{
		private readonly MassiveSparseSet _massiveSparseSet;
		private readonly T[] _dataByFrames;

		public MassiveManagedDataSet(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSparseSet(framesCapacity, dataCapacity))
		{
			// Fetch instance from base
			_massiveSparseSet = (MassiveSparseSet)SparseSet;

			_dataByFrames = new T[framesCapacity * Data.Length];

			for (int i = 0; i < _dataByFrames.Length; i++)
			{
				ComponentMeta<T>.Initialize(out _dataByFrames[i]);
			}
		}

		public int CanRollbackFrames => _massiveSparseSet.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveSparseSet.SaveFrame();

			// We can sync saving with MassiveSparseSet saving, using its CurrentFrame
			int destinationIndex = _massiveSparseSet.CurrentFrame * Data.Length;
			for (int i = 0; i < AliveCount; i++)
			{
				ComponentMeta<T>.Clone(Data[i], ref _dataByFrames[destinationIndex + i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveSparseSet.Rollback(frames);

			// Similarly to saving, we can sync rollback with MassiveSparseSet rollback
			int destinationIndex = _massiveSparseSet.CurrentFrame * Data.Length;
			for (int i = 0; i < AliveCount; i++)
			{
				ComponentMeta<T>.Clone(_dataByFrames[destinationIndex + i], ref Data[i]);
			}
		}
	}
}
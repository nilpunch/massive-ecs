using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveSparseSet : SparseSet, IMassive
	{
		private readonly SparseSetFrames _sparseSetFrames;

		public MassiveSparseSet(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity)
		{
			_sparseSetFrames = new SparseSetFrames(this, framesCapacity);
		}

		public int CurrentFrame => _sparseSetFrames.CurrentFrame;

		public int CanRollbackFrames => _sparseSetFrames.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_sparseSetFrames.SaveFrame();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_sparseSetFrames.Rollback(frames);
		}
	}
}
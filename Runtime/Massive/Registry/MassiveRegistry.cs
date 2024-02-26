using System;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public class MassiveRegistry : RegistryBase<IMassiveSet>, IMassive
	{
		public MassiveRegistry(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSetFactory(framesCapacity, dataCapacity))
		{
		}

		public int CanRollbackFrames => Entities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			foreach (var massive in AllSets)
			{
				massive.SaveFrame();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			foreach (var massive in AllSets)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveSparseSet _entities;

		public MassiveRegistry(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
			: base(new MassiveSetFactory(framesCapacity, dataCapacity))
		{
			// Fetch instance from base
			_entities = (MassiveSparseSet)Entities;
		}

		public int CanRollbackFrames => _entities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in AllSets)
			{
				massive.SaveFrame();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in AllSets)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}
	}
}
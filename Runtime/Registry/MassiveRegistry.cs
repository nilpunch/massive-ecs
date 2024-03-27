using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveIdentifiers _massiveEntities;
		private readonly MassiveGroupsController _massiveGroups;

		public MassiveRegistry(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity, bool storeTagsAsComponents = false)
			: base(new MassiveGroupsController(dataCapacity, framesCapacity), new MassiveSetFactory(dataCapacity, framesCapacity, storeTagsAsComponents))
		{
			// Fetch instances from base
			_massiveEntities = (MassiveIdentifiers)Entities;
			_massiveGroups = (MassiveGroupsController)Groups;
		}

		public int CanRollbackFrames => _massiveEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntities.SaveFrame();
			_massiveGroups.SaveFrame();

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in AllSets)
			{
				massive.SaveFrame();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveEntities.Rollback(frames);
			_massiveGroups.Rollback(frames);

			// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
			foreach (IMassive massive in AllSets)
			{
				massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
			}
		}
	}
}
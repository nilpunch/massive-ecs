using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveEntities _massiveEntityEntities;
		private readonly MassiveGroupsController _massiveGroups;

		public MassiveRegistry(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity, bool storeEmptyTypesAsDataSets = false)
			: base(new MassiveGroupsController(dataCapacity, framesCapacity), new MassiveEntities(dataCapacity, framesCapacity), new MassiveSetFactory(dataCapacity, framesCapacity, storeEmptyTypesAsDataSets))
		{
			// Fetch instances from base
			_massiveEntityEntities = (MassiveEntities)Entities;
			_massiveGroups = (MassiveGroupsController)Groups;
		}

		public int CanRollbackFrames => _massiveEntityEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntityEntities.SaveFrame();
			_massiveGroups.SaveFrame();

			foreach (var set in AllSets)
			{
				if (set is IMassive massive)
				{
					massive.SaveFrame();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveEntityEntities.Rollback(Math.Min(frames, _massiveEntityEntities.CanRollbackFrames));
			_massiveGroups.Rollback(Math.Min(frames, _massiveGroups.CanRollbackFrames));

			foreach (var set in AllSets)
			{
				if (set is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}
	}
}

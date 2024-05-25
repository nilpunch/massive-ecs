using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveEntities _massiveEntities;
		private readonly MassiveGroupsController _massiveGroups;

		public MassiveRegistry(int setCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
			: base(new MassiveGroupsController(setCapacity, framesCapacity), new MassiveEntities(setCapacity, framesCapacity),
				new MassiveSetFactory(setCapacity, framesCapacity, storeEmptyTypesAsDataSets, pageSize))
		{
			// Fetch instances from base
			_massiveEntities = (MassiveEntities)Entities;
			_massiveGroups = (MassiveGroupsController)Groups;
		}

		public int CanRollbackFrames => _massiveEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntities.SaveFrame();
			_massiveGroups.SaveFrame();

			foreach (var set in SetCollection)
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
			_massiveEntities.Rollback(Math.Min(frames, _massiveEntities.CanRollbackFrames));
			_massiveGroups.Rollback(Math.Min(frames, _massiveGroups.CanRollbackFrames));

			foreach (var set in SetCollection)
			{
				if (set is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}
	}
}

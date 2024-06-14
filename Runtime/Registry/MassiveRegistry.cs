using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveEntities _massiveEntities;

		public MassiveRegistry(int setCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
			: base(new MassiveSetFactory(setCapacity, framesCapacity, storeEmptyTypesAsDataSets, pageSize),
				new MassiveGroupFactory(setCapacity, framesCapacity), new MassiveEntities(setCapacity, framesCapacity))
		{
			// Fetch instances from base
			_massiveEntities = (MassiveEntities)Entities;
		}

		public int CanRollbackFrames => _massiveEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntities.SaveFrame();

			for (var i = 0; i < SetRegistry.All.Count; i++)
			{
				if (SetRegistry.All[i] is IMassive massive)
				{
					massive.SaveFrame();
				}
			}

			for (var i = 0; i < GroupRegistry.All.Count; i++)
			{
				if (GroupRegistry.All[i] is IMassive massive)
				{
					massive.SaveFrame();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveEntities.Rollback(Math.Min(frames, _massiveEntities.CanRollbackFrames));

			for (var i = 0; i < SetRegistry.All.Count; i++)
			{
				if (SetRegistry.All[i] is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}

			for (var i = 0; i < GroupRegistry.All.Count; i++)
			{
				if (GroupRegistry.All[i] is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}
	}
}

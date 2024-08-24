using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveEntities _massiveEntities;
		private readonly MassiveBitsetSet _massiveBitsetSet;

		public MassiveRegistry(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
			: base(new MassiveSetFactory(setCapacity, framesCapacity, storeEmptyTypesAsDataSets, pageSize),
				new MassiveGroupFactory(setCapacity, framesCapacity), new MassiveEntities(setCapacity, framesCapacity),
				new MassiveBitsetSet(capacity: setCapacity, framesCapacity: framesCapacity))
		{
			// Fetch instances from base
			_massiveEntities = (MassiveEntities)Entities;
			_massiveBitsetSet = (MassiveBitsetSet)BitsetSet;
		}

		public int CanRollbackFrames => _massiveEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntities.SaveFrame();
			_massiveBitsetSet.SaveFrame();

			var sets = SetRegistry.All;
			for (var i = 0; i < sets.Length; i++)
			{
				if (sets[i] is IMassive massive)
				{
					massive.SaveFrame();
				}
			}

			var groups = GroupRegistry.All;
			for (var i = 0; i < groups.Length; i++)
			{
				if (groups[i] is IMassive massive)
				{
					massive.SaveFrame();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveEntities.Rollback(Math.Min(frames, _massiveEntities.CanRollbackFrames));
			_massiveBitsetSet.Rollback(Math.Min(frames, _massiveBitsetSet.CanRollbackFrames));

			var sets = SetRegistry.All;
			for (var i = 0; i < sets.Length; i++)
			{
				if (sets[i] is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}

			var groups = GroupRegistry.All;
			for (var i = 0; i < groups.Length; i++)
			{
				if (groups[i] is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}
		}
	}
}

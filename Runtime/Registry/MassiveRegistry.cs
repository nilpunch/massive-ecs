using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveEntities _massiveEntities;

		public MassiveRegistry()
			: this(new MassiveRegistryConfig())
		{
		}

		public MassiveRegistry(MassiveRegistryConfig registryConfig)
			: base(new MassiveSetFactory(registryConfig.SetCapacity, registryConfig.FramesCapacity, registryConfig.StoreEmptyTypesAsDataSets, registryConfig.DataPageSize, registryConfig.DefaultPackingMode),
				new MassiveGroupFactory(registryConfig.SetCapacity, registryConfig.FramesCapacity), new MassiveEntities(registryConfig.SetCapacity, registryConfig.FramesCapacity))
		{
			// Fetch instances from base
			_massiveEntities = (MassiveEntities)Entities;
		}

		public event Action FrameSaved;
		public event Action<int> Rollbacked;

		public int CanRollbackFrames => _massiveEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntities.SaveFrame();

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

			FrameSaved?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_massiveEntities.Rollback(Math.Min(frames, _massiveEntities.CanRollbackFrames));

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

			Rollbacked?.Invoke(frames);
		}
	}
}

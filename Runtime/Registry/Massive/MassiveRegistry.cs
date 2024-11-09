using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.Registry"/>.
	/// </summary>
	public class MassiveRegistry : Registry, IMassive
	{
		private readonly MassiveEntities _massiveEntities;

		public MassiveRegistry()
			: this(new MassiveRegistryConfig())
		{
		}

		public MassiveRegistry(MassiveRegistryConfig registryConfig)
			: base(new MassiveSetFactory(registryConfig.FramesCapacity, registryConfig.StoreEmptyTypesAsDataSets, registryConfig.PageSize, registryConfig.FullStability),
				new MassiveReactiveFactory(registryConfig.FramesCapacity), new MassiveEntities(registryConfig.FramesCapacity), registryConfig.PageSize)
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

			var reactiveFilters = ReactiveRegistry.All;
			for (var i = 0; i < reactiveFilters.Length; i++)
			{
				if (reactiveFilters[i] is IMassive massive)
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

			var reactiveFilters = ReactiveRegistry.All;
			for (var i = 0; i < reactiveFilters.Length; i++)
			{
				if (reactiveFilters[i] is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}

			Rollbacked?.Invoke(frames);
		}
	}
}

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.Registry"/>.
	/// </summary>
	public class MassiveRegistry : Registry, IMassive
	{
		public new MassiveEntities Entities { get; }

		public new MassiveRegistryConfig Config { get; }

		public MassiveRegistry()
			: this(new MassiveRegistryConfig())
		{
		}

		public MassiveRegistry(MassiveRegistryConfig registryConfig)
			: base(new MassiveEntities(registryConfig.FramesCapacity), new MassiveSetFactory(registryConfig), new MassiveGroupFactory(registryConfig.FramesCapacity), registryConfig)
		{
			// Fetch instance from base.
			Entities = (MassiveEntities)base.Entities;

			Entities.SaveFrame(); // Save first empty frame so we can rollback to it.

			Config = registryConfig;
		}

		public event Action FrameSaved;
		public event Action<int> Rollbacked;

		public int CanRollbackFrames => Entities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			Entities.SaveFrame();

			foreach (var set in SetRegistry.All)
			{
				if (set is IMassive massive)
				{
					massive.SaveFrame();
				}
			}

			foreach (var group in GroupRegistry.All)
			{
				if (group is IMassive massive)
				{
					massive.SaveFrame();
				}
			}

			FrameSaved?.Invoke();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			Entities.Rollback(frames);

			foreach (var set in SetRegistry.All)
			{
				if (set is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}

			foreach (var group in GroupRegistry.All)
			{
				if (group is IMassive massive)
				{
					massive.Rollback(Math.Min(frames, massive.CanRollbackFrames));
				}
			}

			Rollbacked?.Invoke(frames);
		}
	}
}

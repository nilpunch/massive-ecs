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

		public new MassiveRegistryConfig Config { get; }

		public MassiveRegistry()
			: this(new MassiveRegistryConfig())
		{
		}

		public MassiveRegistry(MassiveRegistryConfig registryConfig)
			: base(new MassiveSetFactory(registryConfig), new MassiveGroupFactory(registryConfig.FramesCapacity),
				new MassiveEntities(registryConfig.FramesCapacity), registryConfig)
		{
			// Fetch instances from base
			_massiveEntities = (MassiveEntities)Entities;

			Config = registryConfig;
		}

		public event Action FrameSaved;
		public event Action<int> Rollbacked;

		public int CanRollbackFrames => _massiveEntities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_massiveEntities.SaveFrame();

			var setList = SetRegistry.All;
			var setCount = setList.Count;
			var sets = setList.Items;
			for (var i = 0; i < setCount; i++)
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

			var setList = SetRegistry.All;
			var setCount = setList.Count;
			var sets = setList.Items;
			for (var i = 0; i < setCount; i++)
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

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="World"/>.
	/// </summary>
	public class MassiveWorld : World, IMassive
	{
		public new MassiveEntities Entities { get; }

		public new MassiveWorldConfig Config { get; }

		public MassiveWorld()
			: this(new MassiveWorldConfig())
		{
		}

		public MassiveWorld(MassiveWorldConfig worldConfig)
			: base(new MassiveEntities(worldConfig.FramesCapacity), new MassiveSetFactory(worldConfig), new MassiveGroupFactory(worldConfig.FramesCapacity), worldConfig)
		{
			// Fetch instance from base.
			Entities = (MassiveEntities)base.Entities;

			Entities.SaveFrame(); // Save first empty frame so we can rollback to it.

			Config = worldConfig;
		}

		public event Action FrameSaved;
		public event Action<int> Rollbacked;

		public int CanRollbackFrames => Entities.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			Entities.SaveFrame();

			foreach (var set in SetRegistry.AllSets)
			{
				if (set is IMassive massive)
				{
					massive.SaveFrame();
				}
			}

			foreach (var group in GroupRegistry.AllGroups)
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

			foreach (var set in SetRegistry.AllSets)
			{
				if (set is IMassive massive)
				{
					massive.Rollback(MathUtils.Min(frames, massive.CanRollbackFrames));
				}
			}

			foreach (var group in GroupRegistry.AllGroups)
			{
				if (group is IMassive massive)
				{
					massive.Rollback(MathUtils.Min(frames, massive.CanRollbackFrames));
				}
			}

			Rollbacked?.Invoke(frames);
		}
	}
}

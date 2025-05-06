using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// Resets data to default value for added elements.
	/// Used for unmanaged components with allocator handles.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class AutoAllocatingDataSet<T> : UnmanagedDataSet<T> where T : unmanaged
	{
		private AutoAllocConfig[] AutoAllocConfigs { get; set; }

		public AutoAllocatingDataSet(AutoAllocConfig[] autoAllocConfigs, int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous,
			T defaultValue = default)
			: base(pageSize, packing, defaultValue)
		{
			AutoAllocConfigs = autoAllocConfigs;
		}

		/// <summary>
		/// Ensures data exists at the specified index, resets it, and allocs handles.
		/// </summary>
		protected override unsafe void EnsureAndPrepareDataAt(int index)
		{
			Data.EnsurePageAt(index);

			ref var data = ref Data[index];
			data = DefaultValue;

			fixed (T* ptr = &data)
			{
				var basePtr = (byte*)ptr;

				var configsLength = AutoAllocConfigs.Length;
				for (var configIndex = 0; configIndex < configsLength; configIndex++)
				{
					var config = AutoAllocConfigs[configIndex];
					var allocator = config.Allocator;
					var handles = config.HandleOffsets;
					var handlesLength = handles.Length;
					for (var handleIndex = 0; handleIndex < handlesLength; handleIndex++)
					{
						var offset = handles[handleIndex];

						var handlePtr = (ChunkId*)(basePtr + offset);
						allocator.TryFree(*handlePtr);
						*handlePtr = allocator.Alloc(0);
					}
				}
			}
		}

		/// <summary>
		/// Resets data at the specified index.
		/// </summary>
		protected override unsafe void PrepareDataAt(int index)
		{
			ref var data = ref Data[index];
			data = DefaultValue;

			fixed (T* ptr = &data)
			{
				var basePtr = (byte*)ptr;

				var configsLength = AutoAllocConfigs.Length;
				for (var configIndex = 0; configIndex < configsLength; configIndex++)
				{
					var config = AutoAllocConfigs[configIndex];
					var allocator = config.Allocator;
					var handles = config.HandleOffsets;
					var handlesLength = handles.Length;
					for (var handleIndex = 0; handleIndex < handlesLength; handleIndex++)
					{
						var offset = handles[handleIndex];

						var handlePtr = (ChunkId*)(basePtr + offset);
						allocator.TryFree(*handlePtr);
						*handlePtr = allocator.Alloc(0);
					}
				}
			}
		}
	}
}

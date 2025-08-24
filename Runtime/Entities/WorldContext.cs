#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct WorldContext
	{
		private readonly SparseSetList _negativeSets;

		public Sets Sets { get; }

		public Masks Masks { get; }

		public Allocators Allocators { get; }

		public WorldContext(Sets sets, Allocators allocators, Masks masks)
		{
			Sets = sets;
			Masks = masks;
			_negativeSets = sets.NegativeSets;
			Allocators = allocators;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EntityDestroyed(int id)
		{
			var buffer = Masks.Buffer;
			var componentCount = Masks.GetAndRemoveAll(id, buffer);

			for (var i = 0; i < componentCount; i++)
			{
				Sets.Lookup[buffer[i]].Remove(id, SparseSet.Update.Nothing);
			}

			Allocators.Free(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EntityCreated(int id)
		{
			var setCount = _negativeSets.Count;
			var sets = _negativeSets.Items;
			for (var i = setCount - 1; i >= 0; i--)
			{
				sets[i].Add(id, SparseSet.Update.Masks);
			}
		}
	}
}

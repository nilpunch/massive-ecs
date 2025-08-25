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
		private SparseSetList NegativeSets { get; }

		public Sets Sets { get; }

		public Masks Masks { get; }

		public Allocators Allocators { get; }

		public WorldContext(Sets sets, Allocators allocators, Masks masks)
		{
			Sets = sets;
			Masks = masks;
			NegativeSets = sets.NegativeSets;
			Allocators = allocators;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EntityDestroyed(int id)
		{
			var buffer = Masks.Buffer;
			var componentCount = Masks.GetAllAndRemove(id, buffer);

			for (var i = 0; i < componentCount; i++)
			{
				Sets.Lookup[buffer[i]].Remove(id, updateMasksAndNegative: false);
			}

			Allocators.Free(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EntityCreated(int id)
		{
			var setCount = NegativeSets.Count;
			var sets = NegativeSets.Items;
			for (var i = setCount - 1; i >= 0; i--)
			{
				sets[i].Add(id, updateMasksAndNegative: false);
				Masks.Set(id, sets[i].ComponentId);
			}
		}
	}
}

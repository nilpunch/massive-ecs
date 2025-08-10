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
		private readonly SparseSetList _allSets;
		private readonly SparseSetList _negativeSets;
		private readonly Allocators _allocators;

		public WorldContext(SparseSetList allSets, SparseSetList negativeSets, Allocators allocators)
		{
			_allSets = allSets;
			_negativeSets = negativeSets;
			_allocators = allocators;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveFromAll(int id)
		{
			var setCount = _allSets.Count;
			var sets = _allSets.Items;
			for (var i = setCount - 1; i >= 0; i--)
			{
				sets[i].Remove(id, updateNegative: false);
			}

			_allocators.Free(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddToNegative(int id)
		{
			var setCount = _negativeSets.Count;
			var sets = _negativeSets.Items;
			for (var i = setCount - 1; i >= 0; i--)
			{
				sets[i].Add(id, updateNegative: false);
			}
		}
	}
}

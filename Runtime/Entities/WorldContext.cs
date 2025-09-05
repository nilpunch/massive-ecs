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
		public BitSets BitSets { get; }

		public Components Components { get; }

		public Allocators Allocators { get; }

		public WorldContext(BitSets bitSets, Allocators allocators, Components components)
		{
			BitSets = bitSets;
			Components = components;
			Allocators = allocators;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EntityDestroyed(int id)
		{
			var buffer = Components.Buffer;
			var componentCount = Components.GetAllAndRemove(id, buffer);

			for (var i = 0; i < componentCount; i++)
			{
				BitSets.Lookup[buffer[i]].Remove(id);
			}

			Allocators.Free(id);
		}
	}
}

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct EntityEnumerable
	{
		private readonly QueryCache _cache;
		private readonly World _world;

		public EntityEnumerable(QueryCache cache, World world)
		{
			_cache = cache;
			_world = world;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerator GetEnumerator()
		{
			return new EntityEnumerator(_cache, _world);
		}
	}
}

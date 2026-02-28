using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class WorldConfig
	{
		public readonly bool StoreEmptyTypesAsDataSets = false;

		public readonly List<ISetSelector> ExcludedImplicitly = new List<ISetSelector>();

		public WorldConfig(bool? storeEmptyTypesAsDataSets = default)
		{
			StoreEmptyTypesAsDataSets = storeEmptyTypesAsDataSets ?? StoreEmptyTypesAsDataSets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(WorldConfig other)
		{
			return StoreEmptyTypesAsDataSets == other.StoreEmptyTypesAsDataSets;
		}

		public WorldConfig ExcludeImplicitly<T>()
		{
			ExcludedImplicitly.Add(new Selector<T>());
			return this;
		}
	}
}

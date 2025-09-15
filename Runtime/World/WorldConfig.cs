using System.Runtime.CompilerServices;

namespace Massive
{
	public class WorldConfig
	{
		public readonly bool StoreEmptyTypesAsDataSets = false;

		public WorldConfig(bool? storeEmptyTypesAsDataSets = default)
		{
			StoreEmptyTypesAsDataSets = storeEmptyTypesAsDataSets ?? StoreEmptyTypesAsDataSets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(WorldConfig other)
		{
			return StoreEmptyTypesAsDataSets == other.StoreEmptyTypesAsDataSets;
		}
	}
}

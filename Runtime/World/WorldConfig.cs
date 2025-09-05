using System.Runtime.CompilerServices;

namespace Massive
{
	public class WorldConfig
	{
		public readonly int PageSize = Constants.DefaultPageSize;

		public readonly bool StoreEmptyTypesAsDataSets = false;

		public readonly bool FullStability = false;

		public WorldConfig(int? pageSize = default, bool? storeEmptyTypesAsDataSets = default, bool? fullStability = default)
		{
			PageSize = pageSize ?? PageSize;
			StoreEmptyTypesAsDataSets = storeEmptyTypesAsDataSets ?? StoreEmptyTypesAsDataSets;
			FullStability = fullStability ?? FullStability;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(WorldConfig other)
		{
			return PageSize == other.PageSize
				&& StoreEmptyTypesAsDataSets == other.StoreEmptyTypesAsDataSets
				&& FullStability == other.FullStability;
		}
	}
}

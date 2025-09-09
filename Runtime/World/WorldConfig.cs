using System.Runtime.CompilerServices;

namespace Massive
{
	public class WorldConfig
	{
		public readonly int PageSize = Constants.DefaultPageSize;

		public readonly bool StoreEmptyTypesAsDataSets = false;

		public WorldConfig(int? pageSize = default, bool? storeEmptyTypesAsDataSets = default)
		{
			PageSize = pageSize ?? PageSize;
			StoreEmptyTypesAsDataSets = storeEmptyTypesAsDataSets ?? StoreEmptyTypesAsDataSets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(WorldConfig other)
		{
			return PageSize == other.PageSize
				&& StoreEmptyTypesAsDataSets == other.StoreEmptyTypesAsDataSets;
		}
	}
}

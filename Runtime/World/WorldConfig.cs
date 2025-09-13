using System.Runtime.CompilerServices;

namespace Massive
{
	public class WorldConfig
	{
		public readonly int PageSize = Constants.DefaultPageSize;

		public readonly bool StoreEmptyTypesAsDataSets = false;

		public readonly bool FullStability = false;

		public readonly Packing PackingWhenIterating = Packing.WithHoles;

		public WorldConfig(int? pageSize = default, bool? storeEmptyTypesAsDataSets = default,
			bool? fullStability = default, Packing? packingWhenIterating = default)
		{
			PageSize = pageSize ?? PageSize;
			StoreEmptyTypesAsDataSets = storeEmptyTypesAsDataSets ?? StoreEmptyTypesAsDataSets;
			FullStability = fullStability ?? FullStability;
			PackingWhenIterating = packingWhenIterating ?? PackingWhenIterating;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(WorldConfig other)
		{
			return PageSize == other.PageSize
				&& StoreEmptyTypesAsDataSets == other.StoreEmptyTypesAsDataSets
				&& FullStability == other.FullStability
				&& PackingWhenIterating == other.PackingWhenIterating;
		}
	}
}

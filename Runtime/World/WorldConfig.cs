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
	}
}

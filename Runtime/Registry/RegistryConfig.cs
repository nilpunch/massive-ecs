namespace Massive
{
	public class RegistryConfig
	{
		public readonly int PageSize = Constants.DefaultPageSize;

		public readonly bool StoreEmptyTypesAsDataSets = false;

		public readonly bool FullStability = false;

		public readonly Packing PackingWhenIterating = Packing.WithHoles;

		public RegistryConfig(int? pageSize = default, bool? storeEmptyTypesAsDataSets = default,
			bool? fullStability = default, Packing? packingWhenIterating = default)
		{
			PageSize = pageSize ?? PageSize;
			StoreEmptyTypesAsDataSets = storeEmptyTypesAsDataSets ?? StoreEmptyTypesAsDataSets;
			FullStability = fullStability ?? FullStability;
			PackingWhenIterating = packingWhenIterating ?? PackingWhenIterating;
		}
	}
}

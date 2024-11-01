namespace Massive
{
	public class RegistryConfig
	{
		public int PageSize = Constants.DefaultPageSize;

		public bool StoreEmptyTypesAsDataSets = false;

		/// <summary>
		/// Enables full stability for component storage. 
		/// This has a minor cost that may balance itself out.
		/// While it can increase iteration count and space consumption,
		/// it eliminates data movements in memory, making component assignment and unassignment faster.
		/// </summary>
		public bool FullStability = false;
	}
}

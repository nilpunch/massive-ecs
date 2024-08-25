namespace Massive
{
	public class RegistryConfig
	{
		public int SetCapacity = Constants.DefaultCapacity;
		public int DataPageSize = Constants.DefaultPageSize;

		public bool StoreEmptyTypesAsDataSets = false;

		public int MaxTypesAmount = Constants.DefaultMaxTypesAmount;

		public bool UseBitsets = false;
		public int BitsetMaxSetsPerEntity = Constants.Bitset.DefaultMaxSetsPerEntity;
		public int BitsetMaxDifferentSets = Constants.Bitset.DefaultMaxDifferentSets;
	}
}

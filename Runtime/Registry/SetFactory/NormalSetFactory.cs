namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _setCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;

		public NormalSetFactory(int setCapacity = Constants.DefaultSetCapacity, bool storeEmptyTypesAsDataSets = false,
			int pageSize = Constants.DefaultPageSize)
		{
			_setCapacity = setCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
		}

		public ISet CreateAppropriateSet<T>()
		{
			if (TypeInfo<T>.HasNoFields && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet();
			}

			return CreateDataSet<T>();
		}

		private ISet CreateSparseSet()
		{
			return new SparseSet(_setCapacity);
		}

		private ISet CreateDataSet<T>()
		{
			return new DataSet<T>(_setCapacity, _pageSize);
		}
	}
}

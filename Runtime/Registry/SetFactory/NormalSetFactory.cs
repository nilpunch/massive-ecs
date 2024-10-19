namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _setCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly PackingMode _defaultPackingMode;

		public NormalSetFactory(int setCapacity = Constants.DefaultCapacity, bool storeEmptyTypesAsDataSets = false,
			int pageSize = Constants.DefaultPageSize, PackingMode defaultPackingMode = PackingMode.Continuous)
		{
			_setCapacity = setCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
			_defaultPackingMode = defaultPackingMode;
		}

		public SparseSet CreateAppropriateSet<T>()
		{
			if (TypeInfo<T>.HasNoFields && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet<T>();
			}

			return CreateDataSet<T>();
		}

		private SparseSet CreateSparseSet<T>()
		{
			return new SparseSet(_setCapacity, IStable.IsImplementedFor<T>() ? PackingMode.WithHoles : _defaultPackingMode);
		}

		private SparseSet CreateDataSet<T>()
		{
			return new DataSet<T>(_setCapacity, _pageSize, IStable.IsImplementedFor<T>() ? PackingMode.WithHoles : _defaultPackingMode);
		}
	}
}

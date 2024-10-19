namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _setCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly bool _fullStability;

		public NormalSetFactory(int setCapacity = Constants.DefaultCapacity, bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize,
			bool fullStability = false)
		{
			_setCapacity = setCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
			_fullStability = fullStability;
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
			return new SparseSet(_setCapacity, GetPackingModeFor<T>());
		}

		private SparseSet CreateDataSet<T>()
		{
			return new DataSet<T>(_setCapacity, _pageSize, GetPackingModeFor<T>());
		}

		private PackingMode GetPackingModeFor<T>()
		{
			return _fullStability || IStable.IsImplementedFor<T>() ? PackingMode.WithHoles : PackingMode.Continuous;
		}
	}
}

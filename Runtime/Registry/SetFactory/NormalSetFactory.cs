﻿namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _setCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;

		public NormalSetFactory(int setCapacity = Constants.DefaultCapacity, bool storeEmptyTypesAsDataSets = false,
			int pageSize = Constants.DefaultPageSize)
		{
			_setCapacity = setCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
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
			return new SparseSet(_setCapacity, IStable.IsImplementedFor<T>() ? PackingMode.WithHoles : PackingMode.Continuous);
		}

		private SparseSet CreateDataSet<T>()
		{
			return new DataSet<T>(_setCapacity, _pageSize, IStable.IsImplementedFor<T>() ? PackingMode.WithHoles : PackingMode.Continuous);
		}
	}
}

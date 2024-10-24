using System;

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
			if (TypeInfo.HasNoFields(typeof(T)) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingModeFor(typeof(T)));
			}

			return CreateDataSet<T>();
		}

		public SparseSet CreateAppropriateSet(Type type)
		{
			if (TypeInfo.HasNoFields(type) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingModeFor(type));
			}

			var args = new object[] { _setCapacity, _pageSize, GetPackingModeFor(type) };
			return (SparseSet)ReflectionHelpers.CreateGeneric(typeof(DataSet<>), type, args);
		}

		private SparseSet CreateSparseSet(PackingMode packingMode)
		{
			return new SparseSet(_setCapacity, packingMode);
		}

		private SparseSet CreateDataSet<T>()
		{
			return new DataSet<T>(_setCapacity, _pageSize, GetPackingModeFor(typeof(T)));
		}

		private PackingMode GetPackingModeFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? PackingMode.WithHoles : PackingMode.Continuous;
		}
	}
}

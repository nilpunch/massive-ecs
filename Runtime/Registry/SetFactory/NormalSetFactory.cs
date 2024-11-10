using System;

namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly bool _fullStability;

		public NormalSetFactory(bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize,
			bool fullStability = false)
		{
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

		public SparseSet CreateAppropriateSetReflected(Type type)
		{
			if (TypeInfo.HasNoFields(type) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingModeFor(type));
			}

			var args = new object[] { _pageSize, GetPackingModeFor(type) };
			return (SparseSet)ReflectionUtils.CreateGeneric(typeof(DataSet<>), type, args);
		}

		private SparseSet CreateSparseSet(PackingMode packingMode)
		{
			return new SparseSet(packingMode);
		}

		private SparseSet CreateDataSet<T>()
		{
			return new DataSet<T>(_pageSize, GetPackingModeFor(typeof(T)));
		}

		private PackingMode GetPackingModeFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? PackingMode.WithHoles : PackingMode.Continuous;
		}
	}
}

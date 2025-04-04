using System;

namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly bool _fullStability;

		public NormalSetFactory(WorldConfig worldConfig)
			: this(worldConfig.StoreEmptyTypesAsDataSets,
				worldConfig.PageSize, worldConfig.FullStability)
		{
		}

		public NormalSetFactory(bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize,
			bool fullStability = false)
		{
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
			_fullStability = fullStability;
		}

		public ISetFactory.Output CreateAppropriateSet<T>()
		{
			var type = typeof(T);

			if (type.IsValueType && ReflectionUtils.HasNoFields(type) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet<T>();
			}

			return CreateDataSet<T>();
		}

		private ISetFactory.Output CreateSparseSet<T>()
		{
			var sparseSet = new SparseSet(GetPackingFor(typeof(T)));
			var cloner = new SparseSetCloner<T>(sparseSet);
			return new ISetFactory.Output(sparseSet, cloner);
		}

		private ISetFactory.Output CreateDataSet<T>()
		{
			if (CopyableUtils.IsImplementedFor(typeof(T)))
			{
				var dataSet = CopyableUtils.CreateCopyingDataSet<T>(_pageSize, GetPackingFor(typeof(T)));
				var cloner = CopyableUtils.CreateCopyingDataSetCloner(dataSet);
				return new ISetFactory.Output(dataSet, cloner);
			}
			else if (typeof(T).IsManaged())
			{
				var dataSet = new SwappingDataSet<T>(_pageSize, GetPackingFor(typeof(T)));
				var cloner = new DataSetCloner<T>(dataSet);
				return new ISetFactory.Output(dataSet, cloner);
			}
			else
			{
				var dataSet = new DataSet<T>(_pageSize, GetPackingFor(typeof(T)));
				var cloner = new DataSetCloner<T>(dataSet);
				return new ISetFactory.Output(dataSet, cloner);
			}
		}

		private Packing GetPackingFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? Packing.WithHoles : Packing.Continuous;
		}
	}
}

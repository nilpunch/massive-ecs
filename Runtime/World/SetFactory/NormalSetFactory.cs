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

		public SparseSet CreateAppropriateSet<T>()
		{
			var type = typeof(T);

			if (type.IsValueType && ReflectionUtils.HasNoFields(type) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingFor(type));
			}

			return CreateDataSet<T>();
		}

		private SparseSet CreateSparseSet(Packing packing)
		{
			return new SparseSet(packing);
		}

		private SparseSet CreateDataSet<T>()
		{
			if (CopyableUtils.IsImplementedFor(typeof(T)))
			{
				return CopyableUtils.CreateCopyingDataSet<T>(_pageSize, GetPackingFor(typeof(T)));
			}
			else if (typeof(T).IsManaged())
			{
				return new SwappingDataSet<T>(_pageSize, GetPackingFor(typeof(T)));
			}
			else
			{
				return new DataSet<T>(_pageSize, GetPackingFor(typeof(T)));
			}
		}

		private Packing GetPackingFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? Packing.WithHoles : Packing.Continuous;
		}
	}
}

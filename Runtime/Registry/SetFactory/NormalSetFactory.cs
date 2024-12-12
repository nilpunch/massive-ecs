using System;

namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly bool _fullStability;

		public NormalSetFactory(RegistryConfig registryConfig)
			: this(registryConfig.StoreEmptyTypesAsDataSets,
				registryConfig.PageSize, registryConfig.FullStability)
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

		public SparseSet CreateAppropriateSetReflected(Type type)
		{
			if (type.IsValueType && ReflectionUtils.HasNoFields(type) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingFor(type));
			}

			var args = new object[] { _pageSize, GetPackingFor(type) };

			if (CopyableUtils.IsImplementedFor(type))
			{
				return (SparseSet)ReflectionUtils.CreateGeneric(typeof(CopyingDataSet<>), type, args);
			}
			else if (type.IsManaged())
			{
				return (SparseSet)ReflectionUtils.CreateGeneric(typeof(SwappingDataSet<>), type, args);
			}
			else
			{
				return (SparseSet)ReflectionUtils.CreateGeneric(typeof(DataSet<>), type, args);
			}
		}

		private SparseSet CreateSparseSet(Packing packing)
		{
			return new SparseSet(packing);
		}

		private SparseSet CreateDataSet<T>()
		{
			if (CopyableUtils.IsImplementedFor(typeof(T)))
			{
				return CopyableUtils.CreateCopyableDataSet<T>(_pageSize, GetPackingFor(typeof(T)));
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

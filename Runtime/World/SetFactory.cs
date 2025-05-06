using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class SetFactory
	{
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly bool _fullStability;

		public SetFactory(WorldConfig worldConfig)
			: this(worldConfig.StoreEmptyTypesAsDataSets,
				worldConfig.PageSize, worldConfig.FullStability)
		{
		}

		public SetFactory(bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize,
			bool fullStability = false)
		{
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
			_fullStability = fullStability;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(SetFactory other)
		{
			return _storeEmptyTypesAsDataSets == other._storeEmptyTypesAsDataSets
				&& _pageSize == other._pageSize
				&& _fullStability == other._fullStability;
		}

		public Output CreateAppropriateSet<T>()
		{
			var type = typeof(T);

			if (TypeHasNoData(type))
			{
				return CreateSparseSet<T>();
			}

			return CreateDataSet<T>();
		}

		private Output CreateSparseSet<T>()
		{
			var sparseSet = new SparseSet(GetPackingFor(typeof(T)));
			var cloner = new SparseSetCloner<T>(sparseSet);
			return new Output(sparseSet, cloner);
		}

		private Output CreateDataSet<T>()
		{
			var type = typeof(T);
			if (CopyableUtils.IsImplementedFor(type))
			{
				var dataSet = CopyableUtils.CreateCopyingDataSet<T>(GetPageSizeFor(type), GetPackingFor(type));
				var cloner = CopyableUtils.CreateCopyingDataSetCloner(dataSet);
				return new Output(dataSet, cloner);
			}
			else if (type.IsManaged())
			{
				var dataSet = new ManagedDataSet<T>(GetPageSizeFor(type), GetPackingFor(type));
				var cloner = new DataSetCloner<T>(dataSet);
				return new Output(dataSet, cloner);
			}
			else
			{
				var dataSet = new UnmanagedDataSet<T>(GetPageSizeFor(type), GetPackingFor(type), DefaultValueUtils.GetDefaultValueFor<T>());
				var cloner = new DataSetCloner<T>(dataSet);
				return new Output(dataSet, cloner);
			}
		}

		public bool TypeHasData(Type type)
		{
			return !TypeHasNoData(type);
		}

		public bool TypeHasNoData(Type type)
		{
			return type.IsValueType && ReflectionUtils.HasNoFields(type) && !_storeEmptyTypesAsDataSets;
		}

		public int GetPageSizeFor(Type type)
		{
			if (Attribute.GetCustomAttribute(type, typeof(PageSizeAttribute)) is PageSizeAttribute attribute)
			{
				return attribute.PageSize;
			}

			return _pageSize;
		}

		public Packing GetPackingFor(Type type)
		{
			return _fullStability || type.IsDefined(typeof(StableAttribute), false) ? Packing.WithHoles : Packing.Continuous;
		}

		public readonly struct Output
		{
			public readonly SparseSet Set;
			public readonly SetCloner Cloner;

			public Output(SparseSet set, SetCloner cloner)
			{
				Set = set;
				Cloner = cloner;
			}

			public void Deconstruct(out SparseSet set, out SetCloner cloner)
			{
				set = Set;
				cloner = Cloner;
			}
		}
	}
}

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
			if (CopyableUtils.IsImplementedFor(typeof(T)))
			{
				var dataSet = CopyableUtils.CreateCopyingDataSet<T>(GetPageSizeFor(typeof(T)), GetPackingFor(typeof(T)));
				var cloner = CopyableUtils.CreateCopyingDataSetCloner(dataSet);
				return new Output(dataSet, cloner);
			}
			else if (typeof(T).IsManaged())
			{
				var dataSet = new SwappingDataSet<T>(GetPageSizeFor(typeof(T)), GetPackingFor(typeof(T)));
				var cloner = new DataSetCloner<T>(dataSet);
				return new Output(dataSet, cloner);
			}
			else
			{
				var dataSet = new DataSet<T>(GetPageSizeFor(typeof(T)), GetPackingFor(typeof(T)));
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

		public Packing GetPackingFor(Type type)
		{
			return _fullStability || type.IsDefined(typeof(StableAttribute), false) ? Packing.WithHoles : Packing.Continuous;
		}

		public int GetPageSizeFor(Type type)
		{
			if (Attribute.GetCustomAttribute(type, typeof(PageSizeAttribute)) is PageSizeAttribute attribute)
			{
				return attribute.PageSize;
			}

			return _pageSize;
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

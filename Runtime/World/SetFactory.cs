using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class SetFactory
	{
		private readonly bool _storeEmptyTypesAsDataSets;

		public SetFactory(WorldConfig worldConfig)
			: this(worldConfig.StoreEmptyTypesAsDataSets)
		{
		}

		public SetFactory(bool storeEmptyTypesAsDataSets = false)
		{
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(SetFactory other)
		{
			return _storeEmptyTypesAsDataSets == other._storeEmptyTypesAsDataSets;
		}

		public Output CreateAppropriateSet<T>()
		{
			var type = typeof(T);

			if (TypeHasNoData(type))
			{
				return CreateBitSet<T>();
			}

			return CreateDataSet<T>();
		}

		private Output CreateBitSet<T>()
		{
			var bitSet = new BitSet();
			var cloner = new BitSetCloner<T>(bitSet);
			return new Output(bitSet, cloner);
		}

		private Output CreateDataSet<T>()
		{
			var type = typeof(T);
			if (CopyableUtils.IsImplementedFor(type))
			{
				var dataSet = CopyableUtils.CreateCopyingDataSet<T>();
				var cloner = CopyableUtils.CreateCopyingDataSetCloner(dataSet);
				return new Output(dataSet, cloner);
			}
			else
			{
				var dataSet = new DataSet<T>(DefaultValueUtils.GetDefaultValueFor<T>());
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

		public readonly struct Output
		{
			public readonly BitSet Set;
			public readonly SetCloner Cloner;

			public Output(BitSet set, SetCloner cloner)
			{
				Set = set;
				Cloner = cloner;
			}

			public void Deconstruct(out BitSet set, out SetCloner cloner)
			{
				set = Set;
				cloner = Cloner;
			}
		}
	}
}

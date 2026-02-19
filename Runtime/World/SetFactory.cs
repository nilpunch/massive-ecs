using System;
using System.Runtime.CompilerServices;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	public class SetFactory
	{
		private readonly Allocator _allocator;
		private readonly bool _storeEmptyTypesAsDataSets;

		public SetFactory(Allocator allocator, WorldConfig worldConfig)
			: this(allocator, worldConfig.StoreEmptyTypesAsDataSets)
		{
		}

		public SetFactory(Allocator allocator, bool storeEmptyTypesAsDataSets = false)
		{
			_allocator = allocator;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CompatibleWith(SetFactory other)
		{
			return _storeEmptyTypesAsDataSets == other._storeEmptyTypesAsDataSets;
		}

		public SetAndCloner CreateAppropriateSet<[Preserve(Member.PublicFields | Member.NonPublicFields | Member.Interfaces)] T>()
		{
			ReflectionUtils.PreserveSize<T>();

			var type = typeof(T);

			if (TypeHasNoData(type))
			{
				return CreateBitSet<T>();
			}

			return CreateDataSet<T>();
		}

		private SetAndCloner CreateBitSet<T>()
		{
			var bitSet = new BitSet();
			var cloner = new BitSetCloner<T>(bitSet);
			return new SetAndCloner(bitSet, cloner);
		}

		private SetAndCloner CreateDataSet<[Preserve(Member.PublicFields | Member.NonPublicFields | Member.Interfaces)] T>()
		{
			var type = typeof(T);
			if (CopyableUtils.IsImplementedFor(type))
			{
				return CopyableUtils.CreateCopyingDataSet(type, Default<T>.Value);
			}
			else if (AutoFreeUtils.IsImplementedFor(type) && AllocatorSchemaGenerator.HasPointers(type))
			{
				return AutoFreeUtils.CreateAutoFreeDataSet(type, _allocator, Default<T>.Value);
			}
			else
			{
				var dataSet = new DataSet<T>(Default<T>.Value);
				var cloner = new DataSetCloner<T>(dataSet);
				return new SetAndCloner(dataSet, cloner);
			}
		}

		public bool TypeHasNoData([Preserve(Member.PublicFields | Member.NonPublicFields)] Type type)
		{
			return type.IsValueType && ReflectionUtils.HasNoFields(type) && !_storeEmptyTypesAsDataSets;
		}
	}
}

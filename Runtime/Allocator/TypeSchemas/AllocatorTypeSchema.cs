#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable StaticMemberInGenericType

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppEagerStaticClassConstruction]
	public static unsafe class AllocatorTypeSchema<T> where T : unmanaged
	{
		public static readonly AllocatorTypeSchema Schema;
		public static readonly bool HasPointers;

		static AllocatorTypeSchema()
		{
			Schema = AllocatorSchemaGenerator.Generate<T>();
			HasPointers = Schema.HasPointers;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeepFree(Allocator allocator, ref T value)
		{
			if (HasPointers)
			{
				fixed (T* ptr = &value)
				{
					Schema.DeepFreePointedData(allocator, (byte*)ptr);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DeepFree(Allocator allocator, Pointer<T> typedPointer)
		{
			var pointer = typedPointer.Raw;

			if (HasPointers)
			{
				var data = allocator.GetPage(pointer).AlignedPtr + pointer.Offset;
				Schema.DeepFreePointedData(allocator, data);
			}

			allocator.Free(pointer);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly unsafe struct AllocatorTypeSchema
	{
		public readonly AllocatorDataSchema[] Schemas;

		public readonly byte RootSchema;

		public bool HasPointers => RootSchema != byte.MaxValue;

		public AllocatorTypeSchema(AllocatorDataSchema[] schemas, byte rootSchema)
		{
			Schemas = schemas;
			RootSchema = rootSchema;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeepFreePointedData(Allocator allocator, byte* data)
		{
			DeepFreePointedData(allocator, data, Schemas[RootSchema]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DeepFreePointedData(Allocator allocator, byte* data, AllocatorDataSchema schema)
		{
			var index = 0;
			var lastPointerFieldOffset = -1;
			while (index < AllocatorDataSchema.Length && schema.OffsetSchemaCount[index] > lastPointerFieldOffset)
			{
				lastPointerFieldOffset = schema.OffsetSchemaCount[index++];

				var nestedPointer = *(Pointer*)(data + (lastPointerFieldOffset & AllocatorDataSchema.UsableMask));

				if (!allocator.IsAllocated(nestedPointer))
				{
					continue;
				}

				var isPrimitive = lastPointerFieldOffset < AllocatorDataSchema.FlagMask;
				if (isPrimitive)
				{
					allocator.Free(nestedPointer);
					continue;
				}

				var nestedData = allocator.GetPtr(nestedPointer);

				var nestedSchemaIndex = schema.OffsetSchemaCount[index++];
				var nestedSchema = Schemas[nestedSchemaIndex & AllocatorDataSchema.UsableMask];

				var isCollection = nestedSchemaIndex >= AllocatorDataSchema.FlagMask;
				if (isCollection)
				{
					var countFieldOffset = schema.OffsetSchemaCount[index++];

					var count = *(int*)(data + countFieldOffset);
					var elementSize = nestedSchema.ElementSize;

					for (var i = 0; i < count; i++)
					{
						DeepFreePointedData(allocator, nestedData, nestedSchema);
						nestedData += elementSize;
					}
				}
				else
				{
					DeepFreePointedData(allocator, nestedData, nestedSchema);
				}

				allocator.Free(nestedPointer);
			}
		}
	}
}

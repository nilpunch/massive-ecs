using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class AllocatorSchemaGenerator
	{
		private static readonly List<AllocatorDataSchema> _schemas = new List<AllocatorDataSchema>();
		private static readonly Dictionary<Type, byte> _schemaIndices = new Dictionary<Type, byte>();
		private static readonly Dictionary<AllocatorDataSchema, byte> _existingSchemas = new Dictionary<AllocatorDataSchema, byte>();

		public static AllocatorTypeSchema Generate<T>() where T : unmanaged
		{
			ClearBuffers();
			var rootSchemaIndex = GetRootSchemaIndex(typeof(T));
			var schema = new AllocatorTypeSchema(_schemas.ToArray(), rootSchemaIndex);
			return schema;
		}

		public static bool HasPointers(Type type)
		{
			return GetPointerFields(type, 0).Any();
		}

		private static void ClearBuffers()
		{
			_schemas.Clear();
			_schemaIndices.Clear();
			_existingSchemas.Clear();
		}

		/// <returns>
		/// <see cref="AllocatorDataSchema.InvalidSchema"/> if type has no pointers.
		/// </returns>
		private static unsafe byte GetRootSchemaIndex(Type type)
		{
			if (_schemaIndices.TryGetValue(type, out var schemaIndex))
			{
				return schemaIndex;
			}

			if (!IsUserStruct(type))
			{
				throw new InvalidOperationException("Type is not user struct.");
			}

			var sizeOfType = ReflectionUtils.SizeOfUnmanaged(type);
			if (sizeOfType > AllocatorDataSchema.MaxElementSize)
			{
				throw new InvalidOperationException($"Size of struct is too big. Size: {sizeOfType}");
			}

			var schema = new AllocatorDataSchema();
			schema.ElementSize = (byte)sizeOfType;

			var complexityCount = 0;
			foreach (var (field, offset) in GetPointerFields(type, 0))
			{
				var fieldType = field.FieldType;
				var pointedType = fieldType.IsGenericType ? fieldType.GetGenericArguments()[0] : null;

				if (complexityCount >= AllocatorDataSchema.Length)
				{
					throw new InvalidOperationException("Struct has too many pointers. Increase PointerSchema.PreferedByteSize or reduce amount of pointers in struct.");
				}

				if (offset > AllocatorDataSchema.MaxOffset)
				{
					throw new InvalidOperationException("Field offset in struct is larger than AllocatorDataSchema.MaxOffset.");
				}

				schema.OffsetSchemaCount[complexityCount++] = (byte)offset;

				if (fieldType == typeof(Pointer))
				{
					continue;
				}

				var nestedSchemaIndex = GetRootSchemaIndex(pointedType);

				if (nestedSchemaIndex > AllocatorDataSchema.MaxSchema)
				{
					continue;
				}

				if (complexityCount >= AllocatorDataSchema.Length)
				{
					throw new InvalidOperationException("Struct has too many pointers. Increase PointerSchema.PreferedByteSize or reduce amount of pointers in struct.");
				}

				schema.OffsetSchemaCount[complexityCount - 1] |= AllocatorDataSchema.FlagMask;
				schema.OffsetSchemaCount[complexityCount++] = nestedSchemaIndex;

				var pointerFieldAttribute = field.GetCustomAttribute<AllocatorPointerFieldAttribute>();
				if (pointerFieldAttribute != null && !string.IsNullOrWhiteSpace(pointerFieldAttribute.CountFieldName))
				{
					if (complexityCount >= AllocatorDataSchema.Length)
					{
						throw new InvalidOperationException("Struct has too many pointers. Increase PointerSchema.PreferedByteSize or reduce amount of pointers in struct.");
					}

					schema.OffsetSchemaCount[complexityCount - 1] |= AllocatorDataSchema.FlagMask;
					schema.OffsetSchemaCount[complexityCount++] = (byte)Marshal.OffsetOf(type, pointerFieldAttribute.CountFieldName);
				}
			}

			if (complexityCount == 0)
			{
				schemaIndex = AllocatorDataSchema.InvalidSchema;
			}
			else if (!_existingSchemas.TryGetValue(schema, out schemaIndex))
			{
				if (_schemas.Count >= AllocatorDataSchema.MaxSchema)
				{
					throw new InvalidOperationException("Too many schemas registered.");
				}

				schemaIndex = (byte)_schemas.Count;
				_schemas.Add(schema);
				_existingSchemas.Add(schema, schemaIndex);
			}

			_schemaIndices.Add(type, schemaIndex);

			return schemaIndex;
		}

		private static IEnumerable<(FieldInfo Field, int Offset)> GetPointerFields(Type type, int baseOffset)
		{
			if (!IsUserStruct(type))
			{
				yield break;
			}

			var fieldWithOffset = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Select(field => (field, GetFieldOffset(type, field)))
				.OrderBy(fieldOffset => fieldOffset.Item2);

			foreach (var (field, offset) in fieldWithOffset)
			{
				var fieldType = field.FieldType;
				var genericTypeDefinition = fieldType.IsGenericType ? fieldType.GetGenericTypeDefinition() : null;

				if (fieldType == typeof(Pointer) || genericTypeDefinition == typeof(Pointer<>))
				{
					yield return (field, offset + baseOffset);
				}
				else if (IsUserStruct(fieldType))
				{
					foreach (var nested in GetPointerFields(fieldType, offset))
					{
						yield return nested;
					}
				}
			}
		}

		private static int GetFieldOffset(Type type, FieldInfo field)
		{
			return (int)Marshal.OffsetOf(type, field.Name);
		}

		private static bool IsUserStruct(Type type)
		{
			return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
		}
	}
}

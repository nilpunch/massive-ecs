using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

// ReSharper disable StaticMemberInGenericType

namespace Massive
{
	public static class HandleOffsetScanner<THandle> where THandle : unmanaged
	{
		private static readonly List<int> s_offsetsBuffer = new List<int>();
		private static readonly Dictionary<Type, int[]> s_cache = new Dictionary<Type, int[]>();

		public static int[] FindHandleOffsets<TComponent>()
		{
			var type = typeof(TComponent);

			if (!s_cache.TryGetValue(type, out var offsets))
			{
				s_offsetsBuffer.Clear();
				ScanType(type, 0, s_offsetsBuffer);
				offsets = s_offsetsBuffer.ToArray();
				s_cache.Add(type, offsets);
			}

			return offsets;
		}

		private static void ScanType(Type type, int offset, List<int> result)
		{
			if (type == typeof(ChunkHandle<THandle>))
			{
				result.Add(offset);
				return;
			}

			foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				var fieldOffset = (int)Marshal.OffsetOf(type, field.Name);
				var fieldType = field.FieldType;

				ScanType(fieldType, offset + fieldOffset, result);
			}
		}
	}
}

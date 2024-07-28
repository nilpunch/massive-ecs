using System;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class ComponentSerializer<T> : IRegistrySerializer where T : unmanaged
	{
		public void Serialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Set<T>();

			SparseSetSerializer.Serialize(set, stream);

			if (set is DataSet<T> dataSet)
			{
				var pagedData = dataSet.Data;
				foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedData.PageSize, set.Count))
				{
					stream.Write(MemoryMarshal.Cast<T, byte>(pagedData.Pages[pageIndex].AsSpan(0, pageLength)));
				}
			}
		}

		public void Deserialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Set<T>();

			SparseSetSerializer.Deserialize(set, stream);

			if (set is DataSet<T> dataSet)
			{
				var pagedData = dataSet.Data;
				foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedData.PageSize, set.Count))
				{
					pagedData.EnsurePage(pageIndex);
					stream.Read(MemoryMarshal.Cast<T, byte>(pagedData.Pages[pageIndex].AsSpan(0, pageLength)));
				}
			}
		}
	}
}

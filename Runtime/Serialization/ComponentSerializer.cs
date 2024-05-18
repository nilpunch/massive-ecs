using System;
using System.IO;

// ReSharper disable MustUseReturnValue
namespace Massive.Serialization
{
	public class ComponentSerializer<T> : IRegistrySerializer where T : unmanaged
	{
		public unsafe void Serialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Any<T>();

			SparseSetSerializer.Serialize(set, stream);

			if (set is DataSet<T> dataSet)
			{
				var pagedData = dataSet.Data;
				foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedData.PageSize, set.Count))
				{
					fixed (T* page = pagedData.Pages[pageIndex])
					{
						stream.Write(new ReadOnlySpan<byte>(page, pageLength * sizeof(T)));
					}
				}
			}
		}

		public unsafe void Deserialize(IRegistry registry, Stream stream)
		{
			var set = (SparseSet)registry.Any<T>();

			SparseSetSerializer.Deserialize(set, stream);

			if (set is DataSet<T> dataSet)
			{
				var pagedData = dataSet.Data;
				foreach (var (pageIndex, pageLength, _) in new PageSequence(pagedData.PageSize, set.Count))
				{
					pagedData.EnsurePage(pageIndex);
					fixed (T* page = pagedData.Pages[pageIndex])
					{
						stream.Read(new Span<byte>(page, pageLength * sizeof(T)));
					}
				}
			}
		}
	}
}

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View : IView
	{
		public IRegistry Registry { get; }

		public View(IRegistry registry)
		{
			Registry = registry;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker
		{
			var entities = Registry.Entities.Alive;
			for (var i = entities.Length - 1; i >= 0; i--)
			{
				invoker.Apply(entities[i].Id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker, T>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>
		{
			var components = Registry.Components<T>();

			var data = components.Data;
			var ids = components.Ids;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, components.Count))
			{
				var page = data.Pages[pageIndex];
				for (int dense = pageLength - 1; dense >= 0; dense--)
				{
					var id = ids[indexOffset + dense];
					invoker.Apply(id, ref page[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker, T1, T2>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2>
		{
			var components1 = Registry.Components<T1>();
			var components2 = Registry.Components<T2>();

			var data1 = components1.Data;
			var data2 = components2.Data;

			// Iterate over smallest data set
			if (components1.Count <= components2.Count)
			{
				var ids1 = components1.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, components1.Count))
				{
					if (!data2.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						int id = ids1[indexOffset + dense1];
						int dense2 = components2.GetDenseOrInvalid(id);
						if (dense2 != Constants.InvalidId)
						{
							invoker.Apply(id, ref page1[dense1], ref data2[dense2]);
						}
					}
				}
			}
			else
			{
				var ids2 = components2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, components2.Count))
				{
					if (!data1.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						int id = ids2[indexOffset + dense2];
						int dense1 = components1.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId)
						{
							invoker.Apply(id, ref data1[dense1], ref page2[dense2]);
						}
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker, T1, T2, T3>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2, T3>
		{
			var components1 = Registry.Components<T1>();
			var components2 = Registry.Components<T2>();
			var components3 = Registry.Components<T3>();

			var data1 = components1.Data;
			var data2 = components2.Data;
			var data3 = components3.Data;

			// Iterate over smallest data set
			if (components1.Count <= components2.Count && components1.Count <= components3.Count)
			{
				var ids1 = components1.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, components1.Count))
				{
					if (!data2.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						int id = ids1[indexOffset + dense1];
						int dense2 = components2.GetDenseOrInvalid(id);
						int dense3 = components3.GetDenseOrInvalid(id);
						if (dense2 != Constants.InvalidId && dense3 != Constants.InvalidId)
						{
							invoker.Apply(id, ref page1[dense1], ref data2[dense2], ref data3[dense3]);
						}
					}
				}
			}
			else if (components2.Count <= components1.Count && components2.Count <= components3.Count)
			{
				var ids2 = components2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, components2.Count))
				{
					if (!data1.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						int id = ids2[indexOffset + dense2];
						int dense1 = components1.GetDenseOrInvalid(id);
						int dense3 = components3.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId && dense3 != Constants.InvalidId)
						{
							invoker.Apply(id, ref data1[dense1], ref page2[dense2], ref data3[dense3]);
						}
					}
				}
			}
			else
			{
				var ids3 = components2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data3.PageSize, components3.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
					{
						continue;
					}

					var page3 = data3.Pages[pageIndex];
					for (int dense3 = pageLength - 1; dense3 >= 0; dense3--)
					{
						int id = ids3[indexOffset + dense3];
						int dense1 = components1.GetDenseOrInvalid(id);
						int dense2 = components2.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId && dense2 != Constants.InvalidId)
						{
							invoker.Apply(id, ref data1[dense1], ref data2[dense2], ref page3[dense3]);
						}
					}
				}
			}
		}
	}
}

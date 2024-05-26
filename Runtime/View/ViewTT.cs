using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View<T1, T2> : IView<T1, T2>
	{
		private readonly IReadOnlyDataSet<T1> _components1;
		private readonly IReadOnlyDataSet<T2> _components2;

		public View(IRegistry registry)
		{
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2>
		{
			var data1 = _components1.Data;
			var data2 = _components2.Data;

			// Iterate over smallest data set
			if (_components1.Count <= _components2.Count)
			{
				var ids1 = _components1.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, _components1.Count))
				{
					if (!data2.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						int id = ids1[indexOffset + dense1];
						if (_components2.TryGetDense(id, out var dense2))
						{
							invoker.Apply(id, ref page1[dense1], ref data2[dense2]);
						}
					}
				}
			}
			else
			{
				var ids2 = _components2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, _components2.Count))
				{
					if (!data1.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						int id = ids2[indexOffset + dense2];
						if (_components1.TryGetDense(id, out var dense1))
						{
							invoker.Apply(id, ref data1[dense1], ref page2[dense2]);
						}
					}
				}
			}
		}
	}
}

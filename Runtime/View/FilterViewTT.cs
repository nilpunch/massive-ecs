using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T1, T2>
	{
		private readonly IFilter _filter;
		private readonly IReadOnlyDataSet<T1> _components1;
		private readonly IReadOnlyDataSet<T2> _components2;

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var minData = SetHelpers.GetMinimalSet(_components1, _components2);
			var ids = SetHelpers.GetMinimalSet(minData, _filter.Include).Ids;

			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, ids.Length))
			{
				if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
				{
					continue;
				}

				var page1 = data1.Pages[pageIndex];
				var page2 = data2.Pages[pageIndex];
				for (int i = pageLength - 1; i >= 0; i--)
				{
					var id = ids[indexOffset + i];
					if (_components1.TryGetDense(id, out var dense1)
					    && _components2.TryGetDense(id, out var dense2)
					    && _filter.ContainsId(id))
					{
						action.Invoke(id, ref page1[dense1], ref page2[dense2]);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, TExtra> action)
		{
			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var minData = SetHelpers.GetMinimalSet(_components1, _components2);
			var ids = SetHelpers.GetMinimalSet(minData, _filter.Include).Ids;

			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, ids.Length))
			{
				if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
				{
					continue;
				}

				var page1 = data1.Pages[pageIndex];
				var page2 = data2.Pages[pageIndex];
				for (int i = pageLength - 1; i >= 0; i--)
				{
					var id = ids[indexOffset + i];
					if (_components1.TryGetDense(id, out var dense1)
					    && _components2.TryGetDense(id, out var dense2)
					    && _filter.ContainsId(id))
					{
						action.Invoke(id, ref page1[dense1], ref page2[dense2], extra);
					}
				}
			}
		}
	}
}

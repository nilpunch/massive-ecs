using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T1, T2> : IView<T1, T2>
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
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2>
		{
			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var minData = SetHelpers.GetMinimalSet(_components1, _components2);
			var ids = SetHelpers.GetMinimalSet(minData, _filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense1 = _components1.GetDenseOrInvalid(id);
				var dense2 = _components2.GetDenseOrInvalid(id);
				if (dense1 != Constants.InvalidId
				    && dense2 != Constants.InvalidId
				    && _filter.ContainsId(id))
				{
					invoker.Apply(id, ref data1[dense1], ref data2[dense2]);
				}
			}
		}
	}
}

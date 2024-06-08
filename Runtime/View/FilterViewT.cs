using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T> : IView<T>
	{
		private readonly IFilter _filter;
		private readonly IReadOnlyDataSet<T> _components;

		public FilterView(IReadOnlyDataSet<T> components, IFilter filter = null)
		{
			_components = components;
			_filter = filter ?? EmptyFilter.Instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>
		{
			var data = _components.Data;
			var ids = SetHelpers.GetMinimalSet(_components, _filter.Include).Ids;

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				var dense = _components.GetDenseOrInvalid(id);
				if (dense != Constants.InvalidId && _filter.ContainsId(id))
				{
					invoker.Apply(id, ref data[dense]);
				}
			}
		}
	}
}

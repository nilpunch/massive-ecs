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

		public FilterView(IRegistry registry, IFilter filter = null)
		{
			_filter = filter ?? EmptyFilter.Instance;
			_components = registry.Components<T>();
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
				if (_components.TryGetDense(id, out var dense)
				    && _filter.ContainsId(id))
				{
					invoker.Apply(id, ref data[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			ForEachUniversal(new EntityActionRefExtraInvoker<T, TExtra> { Action = action, Extra = extra });
		}
	}
}

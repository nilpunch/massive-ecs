using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T> where T : struct
	{
		private readonly IDataSet<T> _components;
		private readonly ISet[] _exclude;
		private readonly ISet[] _componentsAndInclude;

		public FilterView(IRegistry registry, ISet[] include = null, ISet[] exclude = null)
		{
			include ??= Array.Empty<ISet>();
			exclude ??= Array.Empty<ISet>();

			_components = registry.Components<T>();
			_exclude = exclude;
			_componentsAndInclude = new ISet[include.Length + 1];
			_componentsAndInclude[0] = _components;
			include.CopyTo(_componentsAndInclude, 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;
			Span<int> allDense = stackalloc int[_componentsAndInclude.Length];

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (ViewUtils.TryGetAllDense(id, _componentsAndInclude, allDense) && ViewUtils.NotAliveInAll(id, _exclude))
				{
					action.Invoke(id, ref data[allDense[0]]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			var data = _components.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;
			Span<int> dense = stackalloc int[_componentsAndInclude.Length];

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (ViewUtils.TryGetAllDense(id, _componentsAndInclude, dense) && ViewUtils.NotAliveInAll(id, _exclude))
				{
					action.Invoke(id, ref data[dense[0]], extra);
				}
			}
		}
	}
}
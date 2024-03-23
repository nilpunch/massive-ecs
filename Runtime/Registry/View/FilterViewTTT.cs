using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView<T1, T2, T3>
		where T1 : struct
		where T2 : struct
		where T3 : struct
	{
		private readonly IDataSet<T1> _components1;
		private readonly IDataSet<T2> _components2;
		private readonly IDataSet<T3> _components3;
		private readonly ISet[] _exclude;
		private readonly ISet[] _componentsAndInclude;

		public FilterView(IRegistry registry, ISet[] include = null, ISet[] exclude = null)
		{
			include ??= Array.Empty<ISet>();
			exclude ??= Array.Empty<ISet>();

			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_components3 = registry.Components<T3>();
			_exclude = exclude;
			_componentsAndInclude = new ISet[include.Length + 3];
			_componentsAndInclude[0] = _components1;
			_componentsAndInclude[1] = _components2;
			_componentsAndInclude[2] = _components3;
			include.CopyTo(_componentsAndInclude, 3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2, T3> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var data3 = _components3.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;
			Span<int> allDense = stackalloc int[_componentsAndInclude.Length];

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (ViewUtils.TryGetAllDense(id, _componentsAndInclude, allDense) && ViewUtils.NotAliveInAll(id, _exclude))
				{
					action.Invoke(id, ref data1[allDense[0]], ref data2[allDense[1]], ref data3[allDense[2]]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var data3 = _components3.AliveData;
			var ids = ViewUtils.GetMinimalSet(_componentsAndInclude).AliveIds;
			Span<int> allDense = stackalloc int[_componentsAndInclude.Length];

			for (int i = ids.Length - 1; i >= 0; i--)
			{
				int id = ids[i];
				if (ViewUtils.TryGetAllDense(id, _componentsAndInclude, allDense) && ViewUtils.NotAliveInAll(id, _exclude))
				{
					action.Invoke(id, ref data1[allDense[0]], ref data2[allDense[1]], ref data3[allDense[2]], extra);
				}
			}
		}
	}
}
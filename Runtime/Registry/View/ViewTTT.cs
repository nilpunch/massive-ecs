using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct View<T1, T2, T3>
		where T1 : struct
		where T2 : struct
		where T3 : struct
	{
		private readonly IDataSet<T1> _components1;
		private readonly IDataSet<T2> _components2;
		private readonly IDataSet<T3> _components3;

		public View(IRegistry registry)
		{
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_components3 = registry.Components<T3>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2, T3> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var data3 = _components3.AliveData;

			// Iterate over smallest data set
			if (data1.Length <= data2.Length && data1.Length <= data3.Length)
			{
				var ids1 = _components1.AliveIds;
				for (int dense1 = ids1.Length - 1; dense1 >= 0; dense1--)
				{
					int id = ids1[dense1];
					if (_components2.TryGetDense(id, out var dense2) &&
					    _components3.TryGetDense(id, out var dense3))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3]);
					}
				}
			}
			else if (data2.Length <= data1.Length && data2.Length <= data3.Length)
			{
				var ids2 = _components2.AliveIds;
				for (int dense2 = ids2.Length - 1; dense2 >= 0; dense2--)
				{
					int id = ids2[dense2];
					if (_components1.TryGetDense(id, out var dense1) &&
					    _components3.TryGetDense(id, out var dense3))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3]);
					}
				}
			}
			else
			{
				var ids3 = _components2.AliveIds;
				for (int dense3 = ids3.Length - 1; dense3 >= 0; dense3--)
				{
					int id = ids3[dense3];
					if (_components1.TryGetDense(id, out var dense1) &&
					    _components2.TryGetDense(id, out var dense2))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3]);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var data3 = _components3.AliveData;

			// Iterate over smallest data set
			if (data1.Length <= data2.Length && data1.Length <= data3.Length)
			{
				var ids1 = _components1.AliveIds;
				for (int dense1 = ids1.Length - 1; dense1 >= 0; dense1--)
				{
					int id = ids1[dense1];
					if (_components2.TryGetDense(id, out var dense2) &&
					    _components3.TryGetDense(id, out var dense3))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3], extra);
					}
				}
			}
			else if (data2.Length <= data1.Length && data2.Length <= data3.Length)
			{
				var ids2 = _components2.AliveIds;
				for (int dense2 = ids2.Length - 1; dense2 >= 0; dense2--)
				{
					int id = ids2[dense2];
					if (_components1.TryGetDense(id, out var dense1) &&
					    _components3.TryGetDense(id, out var dense3))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3], extra);
					}
				}
			}
			else
			{
				var ids3 = _components2.AliveIds;
				for (int dense3 = ids3.Length - 1; dense3 >= 0; dense3--)
				{
					int id = ids3[dense3];
					if (_components1.TryGetDense(id, out var dense1) &&
					    _components2.TryGetDense(id, out var dense2))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2], ref data3[dense3], extra);
					}
				}
			}
		}
	}
}
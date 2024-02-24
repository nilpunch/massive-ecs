using System.Runtime.CompilerServices;

namespace Massive.Samples.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly struct View<T1, T2>
		where T1 : struct
		where T2 : struct
	{
		private readonly IDataSet<T1> _components1;
		private readonly IDataSet<T2> _components2;

		public View(IDataSet<T1> components1, IDataSet<T2> components2)
		{
			_components1 = components1;
			_components2 = components2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action) => ForEach((int id, ref T1 _, ref T2 _) => action.Invoke(id));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(ActionRef<T1> action) => ForEach((int _, ref T1 value1, ref T2 _) => action.Invoke(ref value1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(ActionRef<T2> action) => ForEach((int _, ref T1 _, ref T2 value2) => action.Invoke(ref value2));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1> action) => ForEach((int id, ref T1 value1, ref T2 _) => action.Invoke(id, ref value1));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T2> action) => ForEach((int id, ref T1 _, ref T2 value2) => action.Invoke(id, ref value2));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(ActionRef<T1, T2> action) => ForEach((int _, ref T1 value1, ref T2 value2) => action.Invoke(ref value1, ref value2));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;

			// Iterate over smallest data set
			if (data1.Length <= data2.Length)
			{
				var ids1 = _components1.AliveIds;
				for (int dense1 = ids1.Length - 1; dense1 >= 0; dense1--)
				{
					int id = ids1[dense1];
					if (_components2.TryGetDense(id, out var dense2))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2]);
					}
				}
			}
			else
			{
				var ids2 = _components2.AliveIds;
				for (int dense2 = ids2.Length - 1; dense2 >= 0; dense2--)
				{
					int id = ids2[dense2];
					if (_components1.TryGetDense(id, out var dense1))
					{
						action.Invoke(id, ref data1[dense1], ref data2[dense2]);
					}
				}
			}
		}
	}
}
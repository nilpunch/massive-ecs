namespace Massive.Samples.ECS
{
	public class MassiveView<T>
		where T : struct
	{
		private readonly MassiveDataSet<T> _massiveData;

		public MassiveView(MassiveDataSet<T> massiveData)
		{
			_massiveData = massiveData;
		}

		public void ForEach(EntityAction action)
		{
			ForEach((int id, ref T value) => action.Invoke(id));
		}
		
		public void ForEach(ActionRef<T> action)
		{
			ForEach((int id, ref T value) => action.Invoke(ref value));
		}

		public void ForEach(EntityActionRef<T> action)
		{
			var massiveData = _massiveData.AliveData;
			var ids = _massiveData.AliveIds;
			for (int i = massiveData.Length - 1; i >= 0; i--)
			{
				action.Invoke(ids[i], ref massiveData[i]);
			}
		}
	}

	public class MassiveView<T1, T2>
		where T1 : struct
		where T2 : struct
	{
		private readonly MassiveDataSet<T1> _massive1;
		private readonly MassiveDataSet<T2> _massive2;

		public MassiveView(MassiveDataSet<T1> massive1)
		{
			_massive1 = massive1;
			_massive1 = massive1;
		}

		public void ForEach(EntityAction action)
		{
			ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(id));
		}
		
		public void ForEach(ActionRef<T1> action)
		{
			ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(ref value1));
		}
		
		public void ForEach(ActionRef<T2> action)
		{
			ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(ref value2));
		}
		
		public void ForEach(EntityActionRef<T1> action)
		{
			ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(id, ref value1));
		}
		
		public void ForEach(EntityActionRef<T2> action)
		{
			ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(id, ref value2));
		}
		
		public void ForEach(ActionRef<T1, T2> action)
		{
			ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(ref value1, ref value2));
		}
		
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			var massiveData1 = _massive1.AliveData;
			var massiveData2 = _massive2.AliveData;

			if (massiveData1.Length <= massiveData2.Length)
			{
				var ids1 = _massive1.AliveIds;
				for (int dense1 = ids1.Length - 1; dense1 >= 0; dense1--)
				{
					int id = ids1[dense1];
					if (_massive2.TryGetDense(id, out var dense2))
					{
						action.Invoke(id, ref massiveData1[dense1], ref massiveData2[dense2]);
					}
				}
			}
			else
			{
				var ids2 = _massive2.AliveIds;
				for (int dense2 = ids2.Length - 1; dense2 >= 0; dense2--)
				{
					int id = ids2[dense2];
					if (_massive1.TryGetDense(id, out var dense1))
					{
						action.Invoke(id, ref massiveData1[dense1], ref massiveData2[dense2]);
					}
				}
			}
		}
	}
}
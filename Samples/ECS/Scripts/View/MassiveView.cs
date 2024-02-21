using Massive;

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

		public void ForEach(ActionRef<T> action)
		{
			var massiveData = _massiveData.AliveData;

			for (int i = massiveData.Length - 1; i >= 0; i--)
			{
				action.Invoke(ref massiveData[i]);
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

		public void ForEach(ActionRef<T1, T2> action)
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
						action.Invoke(ref massiveData1[dense1], ref massiveData2[dense2]);
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
						action.Invoke(ref massiveData1[dense1], ref massiveData2[dense2]);
					}
				}
			}
		}
	}
}
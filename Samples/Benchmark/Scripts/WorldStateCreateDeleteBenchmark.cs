using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateCreateDeleteBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveDataSet<TestState> _massiveData;

		private void Start()
		{
			_massiveData = new MassiveDataSet<TestState>(100, _worldEntitiesCount);
		}

		protected override void Sample()
		{
			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				int temp = _massiveData.Create(new TestState() { Value = index + 1 }).Id;
			}

			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_massiveData.Delete(index);
			}
		}
	}
}
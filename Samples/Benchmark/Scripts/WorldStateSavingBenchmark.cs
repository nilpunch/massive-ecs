using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateSavingBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveDataSet<TestState> _massiveData;

		private void Start()
		{
			_massiveData = new MassiveDataSet<TestState>(100, _worldEntitiesCount);

			for (int i = 0; i < _worldEntitiesCount; i++)
			{
				int temp = _massiveData.Create(new TestState() { Value = 1 + i }).Id;
			}
		}

		protected override void Sample()
		{
			_massiveData.SaveFrame();
		}
	}
}
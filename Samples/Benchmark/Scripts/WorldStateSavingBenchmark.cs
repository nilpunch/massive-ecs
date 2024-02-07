using UnityEngine;

namespace MassiveData.Samples.Benchmark
{
	public class WorldStateSavingBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private Massive<TestState> _massive;

		private void Start()
		{
			_massive = new Massive<TestState>(100, _worldEntitiesCount);

			for (int i = 0; i < _worldEntitiesCount; i++)
			{
				_massive.Create(new TestState() { Value = 1 + i });
			}
		}

		protected override void Sample()
		{
			_massive.SaveFrame();
		}
	}
}
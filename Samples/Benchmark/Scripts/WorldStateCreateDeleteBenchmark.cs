using UnityEngine;

namespace MassiveData.Samples.Benchmark
{
	public class WorldStateCreateDeleteBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private Massive<TestState> _massive;

		private void Start()
		{
			_massive = new Massive<TestState>(100, _worldEntitiesCount);
		}

		protected override void Sample()
		{
			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_massive.Create(new TestState() { Value = index + 1 });
			}

			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_massive.Delete(index);
			}
		}
	}
}
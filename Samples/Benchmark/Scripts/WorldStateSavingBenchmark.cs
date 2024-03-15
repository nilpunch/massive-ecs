using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateSavingBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(_worldEntitiesCount, 121);
		}

		protected override void Sample()
		{
			_registry.SaveFrame();
		}
	}
}
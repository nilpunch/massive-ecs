using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateSavingBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private Registry _registry;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
		}

		protected override void Sample()
		{
			_registry.SaveFrame();
		}
	}
}
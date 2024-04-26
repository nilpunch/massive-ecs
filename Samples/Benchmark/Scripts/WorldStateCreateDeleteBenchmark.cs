using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateCreateDeleteBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(_worldEntitiesCount, 121);
		}

		protected override void Sample()
		{
			var view = new View(_registry);

			view.ForEachExtra(_registry, (id, registry) =>
			{
				registry.Destroy(id);
				registry.Create(new TestState() { Position = Vector3.one });
			});
		}
	}
}

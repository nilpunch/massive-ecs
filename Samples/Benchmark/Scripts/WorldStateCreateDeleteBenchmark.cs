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

			foreach (var id in _registry.Entities)
			{
				_registry.Destroy(id);
			}
		}

		protected override void Sample()
		{
			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_registry.Create(new TestState() { Position = Vector3.one });
			}

			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_registry.Destroy(index);
			}
		}
	}
}
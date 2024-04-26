using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateAddRemoveBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 1000;

		private MassiveRegistry _registry;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(_worldEntitiesCount, 121);
		}

		protected override void Sample()
		{
			var dataSet = _registry.Components<TestState>();

			foreach (var entity in _registry.Entities.Alive)
			{
				dataSet.Assign(entity.Id, new TestState() { Position = Vector3.one });
			}

			foreach (var entity in _registry.Entities.Alive)
			{
				dataSet.Unassign(entity.Id);
			}
		}
	}
}

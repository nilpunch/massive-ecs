using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateAddRemoveBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 1000;
		
		private MassiveRegistry _registry;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
		}

		protected override void Sample()
		{
			var entitiesAliveIds = _registry.Entities.AliveIds;

			foreach (var id in entitiesAliveIds)
			{
				_registry.Add(id, new TestState() { Position = Vector3.one });
			}
			
			foreach (var id in entitiesAliveIds)
			{
				_registry.Remove<TestState>(id);
			}
		}
	}
}
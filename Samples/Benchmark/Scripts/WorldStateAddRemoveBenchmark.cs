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
			foreach (var id in _registry.Entities)
			{
				_registry.Add(id, new TestState() { Position = Vector3.one });
			}
			
			foreach (var id in _registry.Entities)
			{
				_registry.Remove<TestState>(id);
			}
		}
	}
}
using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateAddRemoveBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 1000;
		
		private Registry _registry;
		private View _entities;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
			_entities = new View(_registry.Entities);
		}

		protected override void Sample()
		{
			_entities.ForEach(entity =>
			{
				_registry.Add(entity, new TestState() { Value = entity + 1 });
			});
			
			_entities.ForEach(entity =>
			{
				_registry.Remove<TestState>(entity);
			});
		}
	}
}
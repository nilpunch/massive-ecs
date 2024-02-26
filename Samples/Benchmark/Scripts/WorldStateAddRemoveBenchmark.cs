using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateAddRemoveBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 1000;
		
		private MassiveRegistry _registry;
		private View _entities;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
			_entities = _registry.View();
		}

		protected override void Sample()
		{
			foreach (var entity in _entities)
			{
				entity.Add(new TestState() { Value = entity.Id + 1 });
			}
			
			foreach (var entity in _entities)
			{
				entity.Remove<TestState>();
			}
		}
	}
}
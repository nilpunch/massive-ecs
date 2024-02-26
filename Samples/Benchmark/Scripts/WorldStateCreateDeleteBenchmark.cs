using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateCreateDeleteBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;
		
		private Registry _registry;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
			
			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_registry.Destroy(index);
			}
		}

		protected override void Sample()
		{
			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_registry.Create(new TestState() { Value = index + 1 });
			}

			for (int index = 0; index < _worldEntitiesCount; index++)
			{
				_registry.Destroy(index);
			}
		}
	}
}
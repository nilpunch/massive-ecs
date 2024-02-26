using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutationBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;
		private View<TestState, TestState<float, byte, int>> _testStates;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
			_testStates = _registry.View<TestState, TestState<float, byte, int>>();
		}

		protected override void Sample()
		{
			foreach (var entity in _testStates)
			{
				ref var state = ref entity.Get<TestState>();
				state.Value += 1;
				state.Data1 *= Quaternion.identity;
			}
		}
	}
}
using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutationBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private Registry _registry;
		private View<TestState, TestState<float, byte, int>> _testStates;

		private void Start()
		{
			_registry = BenchmarkUtils.GetFullyPackedRegistry(121, _worldEntitiesCount);
			_testStates = new View<TestState, TestState<float, byte, int>>(_registry.Components<TestState>(), _registry.Components<TestState<float, byte, int>>());
		}

		protected override void Sample()
		{
			_testStates.ForEach((ref TestState state) =>
			{
				state.Value += 1;
				state.Data1 *= Quaternion.identity;
			});
		}
	}
}
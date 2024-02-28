using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutationBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;
		private IDataSet<TestState> _testStates;

		private void Start()
		{
			_registry = BenchmarkUtils.GetSimplyPackedRegistry(1, _worldEntitiesCount);
			_testStates = _registry.Components<TestState>();
		}

		protected override void Sample()
		{
			var data = _testStates.AliveData;
			for (int i = 0; i < data.Length; i++)
			{
				ref TestState state = ref data[i];
				state.Position += Vector3.one;
			}
		}
	}
}
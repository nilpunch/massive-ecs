using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutation2Benchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;
		private IDataSet<TestState> _testStates1;
		private IDataSet<TestState2> _testStates2;

		private void Start()
		{
			_registry = BenchmarkUtils.GetSimplyPackedRegistry(1, _worldEntitiesCount);
			foreach (var id in _registry.Entities.AliveIds)
				_registry.Add<TestState2>(id);

			_testStates1 = _registry.Component<TestState>();
			_testStates2 = _registry.Component<TestState2>();
		}

		protected override void Sample()
		{
			var ids1 = _testStates1.AliveIds;
			var data1 = _testStates1.AliveData;
			var data2 = _testStates2.AliveData;

			for (int dense1 = 0; dense1 < ids1.Length; dense1++)
			{
				int id = ids1[dense1];
				if (_testStates2.TryGetDense(id, out var dense2))
				{
					ref TestState state1 = ref data1[dense1];
					ref TestState2 state2 = ref data2[dense2];
					state1.Position += Vector3.one;
					state2.Position += Vector3.one;
				}
			}
		}
	}
}
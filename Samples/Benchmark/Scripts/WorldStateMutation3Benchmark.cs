using Massive.ECS;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutation3Benchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;
		private DataSet<TestState> _testStates1;
		private DataSet<TestState2> _testStates2;
		private DataSet<TestState3> _testStates3;

		private void Start()
		{
			_registry = BenchmarkUtils.GetSimplyPackedRegistry(1, _worldEntitiesCount);
			foreach (var id in _registry.Entities.AliveIds)
			{
				_registry.Add<TestState2>(id);
				_registry.Add<TestState3>(id);
			}

			_testStates1 = _registry.Component<TestState>();
			_testStates2 = _registry.Component<TestState2>();
			_testStates3 = _registry.Component<TestState3>();
		}

		protected override void Sample()
		{
			var ids1 = _testStates1.AliveIds;
			var data1 = _testStates1.AliveData;
			var data2 = _testStates2.AliveData;
			var data3 = _testStates3.AliveData;

			for (int dense1 = 0; dense1 < ids1.Length; dense1++)
			{
				int id = ids1[dense1];
				if (_testStates2.TryGetDense(id, out var dense2)
				    && _testStates3.TryGetDense(id, out var dense3))
				{
					ref TestState state1 = ref data1[dense1];
					ref TestState2 state2 = ref data2[dense2];
					ref TestState3 state3 = ref data3[dense3];
					state1.Position += Vector3.one;
					state2.Position += Vector3.one;
					state3.Position += Vector3.one;
				}
			}
		}
	}
}
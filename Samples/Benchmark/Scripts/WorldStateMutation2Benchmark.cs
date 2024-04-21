using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutation2Benchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveRegistry _registry;
		private GroupView<TestState, TestState2> _groupView;

		private void Start()
		{
			_registry = BenchmarkUtils.GetSimplyPackedRegistry(_worldEntitiesCount, 1);
			foreach (var entityId in _registry.Entities.Alive)
			{
				_registry.Assign<TestState2>(entityId);
			}

			// Better to cache this
 			IGroup group = _registry.Group(_registry.Many<TestState, TestState2>());

			// Free abstraction, create anywhere you want
			var view = new GroupView<TestState, TestState2>(_registry, group);

			_groupView = view;
		}

		protected override void Sample()
		{
			_groupView.ForEach((int id, ref TestState state1, ref TestState2 state2) =>
			{
				state1.Position += Vector3.one;
				state2.Position += Vector3.one;
			});
		}
	}
}
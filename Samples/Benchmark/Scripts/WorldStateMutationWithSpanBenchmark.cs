using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutationWithSpanBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private WorldState<TestState> _worldState;

		private void Start()
		{
			_worldState = new WorldState<TestState>(100, _worldEntitiesCount);

			var currentFrame = _worldState.CurrentFrame;

			for (var index = 0; index < _worldEntitiesCount; index++)
			{
				currentFrame.Create(new TestState() { Value = index + 1 });
			}
		}

		protected override void Sample()
		{
			var currentFrame = _worldState.CurrentFrame;

			var states = currentFrame.GetAllStates();
			var ids = currentFrame.GetAllIds();
			for (var i = 0; i < currentFrame.AliveCount; i++)
			{
				if (currentFrame.IsAlive(ids[i]))
				{
					ref var state = ref states[i];
					state.Value += 1;
					state.Data1 *= Quaternion.identity;
				}
			}
		}
	}
}
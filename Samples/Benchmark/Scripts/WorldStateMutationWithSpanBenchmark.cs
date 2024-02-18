using UnityEngine;

namespace MassiveData.Samples.Benchmark
{
	public class WorldStateMutationWithSpanBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private Massive<TestState> _massive;

		private void Start()
		{
			_massive = new Massive<TestState>(100, _worldEntitiesCount);

			for (var index = 0; index < _worldEntitiesCount; index++)
			{
				_massive.Create(new TestState() { Value = index + 1 });
			}
		}

		protected override void Sample()
		{
			var states = _massive.AliveData;
			for (var i = 0; i < states.Length; i++)
			{
				ref var state = ref states[i];
				state.Value += 1;
				state.Data1 *= Quaternion.identity;
			}
		}
	}
}
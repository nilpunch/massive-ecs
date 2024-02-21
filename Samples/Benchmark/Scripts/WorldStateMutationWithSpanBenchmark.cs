using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutationWithSpanBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveDataSet<TestState> _massiveData;

		private void Start()
		{
			_massiveData = new MassiveDataSet<TestState>(100, _worldEntitiesCount);

			for (var index = 0; index < _worldEntitiesCount; index++)
			{
				_massiveData.Create(new TestState() { Value = index + 1 });
			}
		}

		protected override void Sample()
		{
			var states = _massiveData.AliveData;
			for (var i = 0; i < states.Length; i++)
			{
				ref var state = ref states[i];
				state.Value += 1;
				state.Data1 *= Quaternion.identity;
			}
		}
	}
}
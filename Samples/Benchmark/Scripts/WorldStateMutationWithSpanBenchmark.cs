using System;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
	public class WorldStateMutationWithSpanBenchmark : MonoProfiler
	{
		[SerializeField, Min(1)] private int _worldEntitiesCount = 100;

		private MassiveData<TestState> _massiveData;

		private void Start()
		{
			_massiveData = new MassiveData<TestState>(100, _worldEntitiesCount);

			for (var index = 0; index < _worldEntitiesCount; index++)
			{
				_massiveData.Create(new TestState() { Value = index + 1 });
			}
		}

		protected override void Sample()
		{
			var states = _massiveData.Data;
			var ids = _massiveData.Sparse;
			for (var i = 0; i < _massiveData.AliveCount; i++)
			{
				if (_massiveData.IsAlive(ids[i]))
				{
					ref var state = ref states[i];
					state.Value += 1;
					state.Data1 *= Quaternion.identity;
				}
			}
		}
	}
}
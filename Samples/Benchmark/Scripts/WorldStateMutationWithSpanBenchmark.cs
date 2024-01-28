using System;
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
        }

        protected override void Sample()
        {
            Span<TestState> states = _worldState.GetAll();
            for (int index = 0; index < states.Length; index++)
            {
                ref TestState state = ref states[index];
                state.Value += 1;
            }
        }
    }
}

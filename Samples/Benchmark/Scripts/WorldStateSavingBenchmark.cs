using System.Collections.Generic;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
    public class WorldStateSavingBenchmark : MonoProfiler
    {
        [SerializeField, Min(1)] private int _worldEntitiesCount = 100;

        private WorldState<TestState> _worldState;

        private void Start()
        {
            _worldState = new WorldState<TestState>(100, _worldEntitiesCount);

            for (int i = 0; i < _worldEntitiesCount; i++)
            {
                _worldState.Create(i, new TestState() { Value = 1 + i});
            }
        }

        protected override void Sample()
        {
            _worldState.SaveFrame();
        }
    }
}

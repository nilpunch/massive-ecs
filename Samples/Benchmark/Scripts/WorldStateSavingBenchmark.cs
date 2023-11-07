using System.Collections.Generic;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
    public class WorldStateSavingBenchmark : MonoProfiler
    {
        [SerializeField, Min(1)] private int _worldEntitiesCount = 100;

        private struct TestState
        {
            public int Value;

            public Vector3 Data1;
            public Quaternion Data2;

            public TestState(int value)
            {
                Value = value;
                Data1 = default;
                Data2 = default;
            }
        }

        private WorldState<TestState> _worldState;

        private void Start()
        {
            _worldState = new WorldState<TestState>(100, _worldEntitiesCount);

            for (int i = 0; i < _worldEntitiesCount; i++)
            {
                _worldState.Reserve(new TestState(i));
            }
        }

        protected override void Sample()
        {
            _worldState.SaveFrame();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Massive.Samples.Benchmark
{
    public class WorldStateSavingBenchmark : MonoProfiler
    {
        [SerializeField, Min(1)] private int _worldEntitiesCount = 100;

        private struct TestState : IState
        {
            public int Value;

            public Vector3 Data1;
            public Quaternion Data3;
            public Quaternion Data4;
            public Quaternion Data5;
            public Quaternion Data6;
            public Quaternion Data7;

            public int SparseIndex { get; set; }
        }

        private WorldState<TestState> _worldState;

        private void Start()
        {
            _worldState = new WorldState<TestState>(100, _worldEntitiesCount);

            for (int i = 0; i < _worldEntitiesCount; i++)
            {
                _worldState.Create(new TestState() { Value = 1 + i});
            }
        }

        protected override void Sample()
        {
            _worldState.SaveFrame();
        }
    }
}

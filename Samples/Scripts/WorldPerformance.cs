using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Massive.Samples
{
    public struct TestState
    {
        public int Value;
        public Vector3 Position;
        public Quaternion Rotation;

        public TestState(int value)
        {
            Value = value;
            Position = default;
            Rotation = default;
        }
    }

    public class WorldPerformance : MonoBehaviour
    {
        private void Start()
        {
            // var entities = FindObjectsOfType<TestEntity>();
            // var entityWorldState = new WorldState<TestEntityState>();

            var testState = new WorldState<int>(frames: 3, statesPerFrame: 3);

            // testState.Reserve(new TestState(1));
            // testState.Reserve(new TestState(2));
            // testState.Reserve(new TestState(3));

            List<StateHandle<int>> stateHandles = new List<StateHandle<int>>();

            for (int i = 0; i < 3; i++)
            {
                stateHandles.Add(testState.Reserve(i + 1));
            }

            testState.SaveFrame();

            for (int i = 0; i < 3; i++)
            {
                stateHandles.ForEach(state => state.State += 3);
                testState.SaveFrame();
            }

            testState.Rollback(3);

            for (int i = 0; i < 3; i++)
            {
                stateHandles.ForEach(state => state.State += 3);
                testState.SaveFrame();
            }

            return;

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int k = 0; k < 1; ++k)
            {
                // for (int i = 0; i < 240; i++)
                // {
                //     foreach (var stateHandle in stateHandles)
                //     {
                //         ref var modified = ref stateHandle.State;
                //         modified.Position += Vector3.right;
                //         modified.Rotation *= Quaternion.identity;
                //     }
                //
                //     testState.SaveFrame();
                // }
            }

            Debug.Log(stopwatch.ElapsedMilliseconds / 100f + "ms");
        }
    }
}

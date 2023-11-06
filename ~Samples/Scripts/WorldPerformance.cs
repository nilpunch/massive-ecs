using System.Collections;
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

            var testState = new WorldState<TestState>(maxFrames: 120, maxStatesPerFrame: 155);

            // testState.Reserve(new TestState(1));
            // testState.Reserve(new TestState(2));
            // testState.Reserve(new TestState(3));

            List<StateHandle<TestState>> stateHandles = new List<StateHandle<TestState>>();

            for (int i = 0; i < 100; i++)
            {
                stateHandles.Add(testState.Reserve(new TestState(i + 1)));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int k = 0; k < 1; ++k)
            {
                for (int i = 0; i < 240; i++)
                {
                    foreach (var stateHandle in stateHandles)
                    {
                        ref var modified = ref stateHandle.State;
                        modified.Position += Vector3.right;
                        modified.Rotation *= Quaternion.identity;
                    }

                    testState.SaveFrame();
                }
            }

            Debug.Log(stopwatch.ElapsedMilliseconds / 100f + "ms");
        }
    }
}

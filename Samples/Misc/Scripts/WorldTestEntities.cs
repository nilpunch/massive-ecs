using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Massive.Samples.Misc
{
    public class WorldTestEntities : MonoBehaviour
    {
        [SerializeField] private float _simulationSpeed = 1f;
        [SerializeField] private int _framesCapacity = 240;
        [SerializeField] private int _resimulate = 0;

        private WorldState<TestEntityState> _worldState;
        private WorldTime _worldTime;
        private List<TestEntity> _testEntities;

        private void Start()
        {
            _testEntities = FindObjectsOfType<TestEntity>().ToList();
            _worldState = new WorldState<TestEntityState>(frames: _framesCapacity, statesPerFrame: _testEntities.Count);

            _worldTime = new WorldTime(60);

            foreach (TestEntity testEntity in _testEntities)
            {
                testEntity.Construct(
                    _worldState.Reserve(new TestEntityState(testEntity.transform.position, testEntity.transform.rotation)),
                    _worldTime);
            }

            _worldState.SaveFrame();
        }

        [field: SerializeField] public float ElapsedTime { get; set; }

        public int TargetTick => Mathf.FloorToInt(ElapsedTime * _worldTime.TicksPerSecond);
        public int CurrentTick { get; set; }
        private int EarliestApprovedTick { get; set; }

        private void Update()
        {
            ElapsedTime += Time.deltaTime * _simulationSpeed;
            ElapsedTime = Mathf.Max(ElapsedTime, 0);

            EarliestApprovedTick = Mathf.Max(TargetTick - _resimulate, 0);

            FastForward(TargetTick);

            void FastForward(int targetTick)
            {
                int earliestTick = Math.Min(targetTick, EarliestApprovedTick);
                int stepsToRollback = Math.Max(CurrentTick - earliestTick, 0);

                _worldState.Rollback(stepsToRollback);
                CurrentTick -= stepsToRollback;

                while (CurrentTick < targetTick)
                {
                    Span<TestEntityState> states = _worldState.GetAll();
                    for (int index = 0; index < states.Length; index++)
                    {
                        ref TestEntityState state = ref states[index];
                        // state.Position += Vector3.forward * (0.5f * _worldTime.DeltaTime);
                        _testEntities[index].Tick(ref state);
                    }

                    _worldState.SaveFrame();
                    CurrentTick += 1;
                }

                EarliestApprovedTick = CurrentTick;
            }
        }
    }
}

using System;
using UnityEngine;

namespace Massive.Samples.Shooter
{
    public class WorldTimeline
    {
        private readonly IWorldState _worldState;
        private readonly ISimulation _simulation;

        public WorldTimeline(IWorldState worldState, ISimulation simulation)
        {
            _worldState = worldState;
            _simulation = simulation;
        }

        private int CurrentFrame { get; set; }

        private int EarliestApprovedTick { get; set; }

        public void UpdateEarliestApprovedFrame(int tick)
        {
            if (EarliestApprovedTick > tick)
            {
                EarliestApprovedTick = tick;
            }
        }

        public void FastForwardToFrame(int targetFrame)
        {
            if (targetFrame < 0)
                throw new ArgumentOutOfRangeException(nameof(targetFrame), "Target frame should not be negative!");

            int earliestTick = Math.Min(targetFrame, EarliestApprovedTick);
            int framesToRollback = Math.Max(CurrentFrame - earliestTick, 0);

            _worldState.Rollback(framesToRollback);
            CurrentFrame -= framesToRollback;

            while (CurrentFrame < targetFrame)
            {
                _simulation.StepForward();
                _worldState.SaveFrame();
                CurrentFrame += 1;
            }

            EarliestApprovedTick = CurrentFrame;
        }
    }
}

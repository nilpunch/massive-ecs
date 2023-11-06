using UnityEngine;

namespace Massive.Samples
{
    public class TestEntity : MonoBehaviour, ITickable
    {
        private StateHandle<TestEntityState> _stateHandle;
        private WorldTime _worldTime;

        public void Construct(StateHandle<TestEntityState> stateHandle, WorldTime worldTime)
        {
            _worldTime = worldTime;
            _stateHandle = stateHandle;
        }

        public void Tick()
        {
            ref TestEntityState state = ref _stateHandle.State;
            state.Position += Vector3.forward * (0.5f * _worldTime.DeltaTime);
        }

        private void LateUpdate()
        {
            ref TestEntityState state = ref _stateHandle.State;
            transform.position = state.Position;
            transform.rotation = state.Rotation;
        }
    }
}

using UnityEngine;

namespace Massive.Samples
{
    public struct TestEntityState
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TestEntityState(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}

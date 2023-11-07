using UnityEngine;

namespace Massive.Samples.Misc
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

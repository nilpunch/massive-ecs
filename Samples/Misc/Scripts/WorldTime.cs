namespace Massive.Samples.Misc
{
    public class WorldTime : ITickable
    {
        public float Time => DeltaTime * Frame;

        public float DeltaTime { get; }

        public int Frame { get; private set; }

        public int FramesPerSecond { get; }

        public WorldTime(int framesPerSecond)
        {
            FramesPerSecond = framesPerSecond;
            DeltaTime = 1f / FramesPerSecond;
        }

        void ITickable.Tick()
        {
            Frame += 1;
        }
    }
}

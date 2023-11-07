namespace Massive.Samples.Misc
{
    public class WorldTime : ITickable
    {
        public float Time => DeltaTime * Tick;

        public float DeltaTime { get; }

        public int Tick { get; private set; }

        public int TicksPerSecond { get; }

        public WorldTime(int ticksPerSecond)
        {
            TicksPerSecond = ticksPerSecond;
            DeltaTime = 1f / TicksPerSecond;
        }

        void ITickable.Tick()
        {
            Tick += 1;
        }
    }
}

namespace Massive.Samples.Shooter
{
    public interface IEntity<TState> where TState : struct
    {
        void UpdateState(ref TState state);
        void Enable();
        void Disable();
    }
}

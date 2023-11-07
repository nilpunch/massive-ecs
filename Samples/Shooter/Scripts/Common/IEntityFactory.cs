namespace Massive.Samples.Shooter
{
    public interface IEntityFactory<TState> where TState : struct
    {
        IEntity<TState> Create();
    }
}
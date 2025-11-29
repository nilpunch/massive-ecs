namespace Massive
{
	public class SystemBase<TState> : ISystem where TState : unmanaged
	{
		public World World { get; private set; }
		public int Id { get; private set; }
		public Allocator Allocator { get; private set; }

		private Pointer<TState> StatePointer { get; set; }

		public ref TState State => ref StatePointer.Value(Allocator);

		public virtual void Initialize(World world, int id, Allocator allocator)
		{
			World = world;
			Id = id;
			Allocator = allocator;
			StatePointer = (Pointer<TState>)Allocator.Alloc<TState>(1);
		}
	}
}

namespace Massive
{
	public abstract class AllocatorCloner
	{
		public abstract void CopyTo(AllocatorRegistry allocatorRegistry);
	}

	public sealed class AllocatorCloner<T> : AllocatorCloner
	{
		private readonly Allocator<T> _allocator;

		public AllocatorCloner(Allocator<T> allocator)
		{
			_allocator = allocator;
		}

		public override void CopyTo(AllocatorRegistry allocatorRegistry)
		{
			_allocator.CopyTo((Allocator<T>)allocatorRegistry.Get<T>());
		}
	}
}

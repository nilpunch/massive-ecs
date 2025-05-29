using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class AllocatorCloner<T> : AllocatorCloner where T : unmanaged
	{
		private readonly Allocator<T> _allocator;

		public AllocatorCloner(Allocator<T> allocator)
		{
			_allocator = allocator;
		}

		public override void CopyTo(Allocators allocators)
		{
			_allocator.CopyTo((Allocator<T>)allocators.Get<T>());
		}
	}

	public abstract class AllocatorCloner
	{
		public abstract void CopyTo(Allocators allocators);
	}
}

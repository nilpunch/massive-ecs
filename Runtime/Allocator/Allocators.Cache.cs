using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class Allocators
	{
		/// <summary>
		/// Non-deterministic, used for lookups.<br/>
		/// Don't store it in simulation.
		/// </summary>
		public int IntId { get; } = AllocatorId<int>.Index;

		public Allocator<int> IntAllocator { get; }

		public Allocators()
		{
			IntAllocator = (Allocator<int>)Get<int>();
		}
	}
}

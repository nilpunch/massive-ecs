using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SystemBase<TState> : ISystem where TState : unmanaged
	{
		public World World { get; private set; }
		public int Id { get; private set; }
		public Allocator Allocator { get; private set; }

		private Pointer<TState> StatePointer { get; set; }

		public ref TState State
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref StatePointer.Value(Allocator);
		}

		void ISystem.Initialize(int id, Allocator allocator, World world)
		{
			Id = id;
			Allocator = allocator;
			World = world;
			StatePointer = (Pointer<TState>)Allocator.Alloc<TState>(1);
		}
	}
}

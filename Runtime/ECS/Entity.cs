using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	public readonly struct Entity
	{
		private readonly Registry _owner;

		public readonly int Id;

		public Entity(Registry registry, int id)
		{
			_owner = registry;
			Id = id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy()
		{
			_owner.Destroy(Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Add<T>(T data = default) where T : unmanaged
		{
			_owner.Add(Id, data);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity Remove<T>() where T : unmanaged
		{
			_owner.Remove<T>(Id);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>() where T : unmanaged
		{
			return _owner.Has<T>(Id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>() where T : unmanaged
		{
			return ref _owner.Get<T>(Id);
		}
	}
}
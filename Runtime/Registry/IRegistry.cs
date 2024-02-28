namespace Massive.ECS
{
	public interface IRegistry
	{
		ISet Entities { get; }
		int Create();
		void Destroy(int entityId);
		ref T Get<T>(int entityId) where T : unmanaged;
		bool Has<T>(int entityId) where T : unmanaged;
		void Add<T>(int entityId, T data = default) where T : unmanaged;
		void Remove<T>(int entityId) where T : unmanaged;
		DataSet<T> Component<T>() where T : unmanaged;
		SparseSet Tag<T>() where T : unmanaged;
		ISet AnySet<T>() where T : unmanaged;
	}
}
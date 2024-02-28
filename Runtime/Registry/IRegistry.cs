namespace Massive.ECS
{
	public interface IRegistry
	{
		SparseSet Entities { get; }
		int Create();
		void Destroy(int entityId);
		void Add<T>(int entityId, T data = default) where T : unmanaged;
		void Remove<T>(int entityId) where T : unmanaged;
		bool Has<T>(int entityId) where T : unmanaged;
		ref T Get<T>(int entityId) where T : unmanaged;
		DataSet<T> Components<T>() where T : unmanaged;
		SparseSet Tags<T>() where T : unmanaged;
		ISet AnySet<T>() where T : unmanaged;
	}
}
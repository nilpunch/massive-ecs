namespace Massive
{
	public interface IRegistry
	{
		Identifiers Entities { get; }
		int Create();
		void Destroy(int entityId);
		void Add<T>(int entityId, T data = default) where T : struct;
		void Remove<T>(int entityId) where T : struct;
		bool Has<T>(int entityId) where T : struct;
		ref T Get<T>(int entityId) where T : struct;
		IDataSet<T> Components<T>() where T : struct;
		ISet Tags<T>() where T : struct;
		ISet AnySet<T>() where T : struct;
	}
}
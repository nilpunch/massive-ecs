using System.Diagnostics.Contracts;

namespace Massive
{
	public interface IRegistry
	{
		[Pure] Identifiers Entities { get; }

		int Create();

		void Destroy(int entityId);

		void Add<T>(int entityId, T data = default) where T : struct;

		void Remove<T>(int entityId) where T : struct;

		[Pure] bool Has<T>(int entityId) where T : struct;

		[Pure] ref T Get<T>(int entityId) where T : struct;

		[Pure] IDataSet<T> Components<T>() where T : struct;

		[Pure] ISet Tags<T>() where T : struct;

		[Pure] ISet AnySet<T>() where T : struct;
	}
}
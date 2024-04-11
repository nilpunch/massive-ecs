using System;

namespace Massive
{
	public interface IRegistry
	{
		IGroupsController Groups { get; }
		Entities Entities { get; }

		event Action<ISet> SetCreated;

		int Create();

		void Destroy(int id);

		bool IsAlive(int id);

		void Add<T>(int id, T data = default) where T : struct;

		void Remove<T>(int id) where T : struct;

		bool Has<T>(int id) where T : struct;

		ref T Get<T>(int id) where T : struct;

		IDataSet<T> Components<T>() where T : struct;

		ISet Any<T>() where T : struct;
	}
}
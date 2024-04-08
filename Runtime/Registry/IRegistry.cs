using System;
using System.Collections.Generic;

namespace Massive
{
	public interface IRegistry
	{
		IGroupsController Groups { get; }
		ISetFactory SetFactory { get; }
		Dictionary<Type, ISet> SetsLookup { get; }
		List<ISet> AllSets { get; }
		Entities Entities { get; }

		Entity Create();

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
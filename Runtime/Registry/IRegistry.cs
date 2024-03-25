using System;
using System.Collections.Generic;

namespace Massive
{
	public interface IRegistry
	{
		Dictionary<Type, ISet> SetsLookup { get; }
		List<ISet> AllSets { get; }
		Identifiers Entities { get; }

		int Create();

		void Destroy(int entityId);

		void Add<T>(int entityId, T data = default) where T : struct;

		void Remove<T>(int entityId) where T : struct;

		bool Has<T>(int entityId) where T : struct;

		ref T Get<T>(int entityId) where T : struct;

		IDataSet<T> Components<T>() where T : struct;

		ISet Any<T>() where T : struct;
	}
}
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
		Identifiers Entities { get; }

		Identifier Create();

		void Destroy(int entityId);
		void Destroy(Identifier identifier);

		bool IsAlive(int entityId);
		bool IsAlive(Identifier identifier);

		void Add<T>(int entityId, T data = default) where T : struct;
		void Add<T>(Identifier identifier, T data = default) where T : struct;

		void Remove<T>(int entityId) where T : struct;
		void Remove<T>(Identifier identifier) where T : struct;

		bool Has<T>(int entityId) where T : struct;
		bool Has<T>(Identifier identifier) where T : struct;

		ref T Get<T>(int entityId) where T : struct;
		ref T Get<T>(Identifier identifier) where T : struct;

		IDataSet<T> Components<T>() where T : struct;

		ISet Any<T>() where T : struct;
	}
}
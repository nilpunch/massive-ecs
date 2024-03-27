using System;
using System.Collections.Generic;

namespace Massive
{
	public interface IGroup
	{
		IFilter Filter { get; }

		ISet[] Owned { get; }

		IReadOnlySet[] Other { get; }

		List<IGroup> Children { get; }

		ReadOnlySpan<int> GroupIds { get; }

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);

		bool IsSubsetOf(IGroup group);

		void AddEntity(int entityId);

		void RemoveEntity(int entityId);
	}
}
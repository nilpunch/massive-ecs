using System;
using System.Collections.Generic;

namespace Massive
{
	public interface IGroup
	{
		IFilter Filter { get; }

		ISet[] Owned { get; }

		IReadOnlySet[] Other { get; }

		IGroup ExtendedGroup { get; set; }

		ReadOnlySpan<int> GroupIds { get; }

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);

		bool ExtendsGroup(IGroup group);
		
		bool ExtendsGroup(ISet[] owned, IReadOnlySet[] other, IFilter filter);

		bool BaseForGroup(ISet[] owned, IReadOnlySet[] other, IFilter filter);
		
		void AddEntity(int entityId);

		void RemoveEntity(int entityId);
	}
}
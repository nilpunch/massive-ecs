using System;

namespace Massive
{
	public interface IGroup
	{
		ReadOnlySpan<int> GroupIds { get; }

		void EnsureSynced();

		bool IsOwning(IReadOnlySet set);

		void AddEntity(int entityId);
		
		void RemoveEntity(int entityId);
	}
}
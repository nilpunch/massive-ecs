using System;

namespace Massive
{
	public interface IGroup
	{
		ReadOnlySpan<int> GroupIds { get; }

		int Length { get; }

		void EnsureSynced();

		bool IsOwning(ISet set);
	}
}
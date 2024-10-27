using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IGroup : IIdsSource
	{
		SparseSet MainSet { get; }

		bool IsSynced { get; }

		void EnsureSynced();

		void Desync();

		bool IsOwning(SparseSet set);

		int[] IIdsSource.Ids
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => MainSet.Ids;
		}
	}
}

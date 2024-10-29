namespace Massive
{
	public interface IGroup
	{
		SparseSet MainSet { get; }

		int Count { get; }

		bool IsSynced { get; }

		void EnsureSynced();

		void Desync();

		bool IsOwning(SparseSet set);
	}
}

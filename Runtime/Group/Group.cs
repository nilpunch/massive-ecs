namespace Massive
{
	public abstract class Group
	{
		public int Count { get; protected set; }
		public SparseSet MainSet { get; protected set; }

		public abstract bool IsSynced { get; protected set; }
		public abstract void EnsureSynced();
		public abstract void Desync();
		public abstract bool IsOwning(SparseSet set);
	}
}

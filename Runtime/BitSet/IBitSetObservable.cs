namespace Massive
{
	public interface IBitSetObservable
	{
		void PushRemoveOnAdd(BitSet bitSet);
		void PopRemoveOnAdd();

		void PushRemoveOnRemove(BitSet bitSet);
		void PopRemoveOnRemove();
	}
}

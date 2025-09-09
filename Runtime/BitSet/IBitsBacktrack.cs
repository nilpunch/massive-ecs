namespace Massive
{
	public interface IBitsBacktrack
	{
		void PushRemoveOnAdd(Bits bits);
		void PopRemoveOnAdd();

		void PushRemoveOnRemove(Bits bits);
		void PopRemoveOnRemove();
	}
}

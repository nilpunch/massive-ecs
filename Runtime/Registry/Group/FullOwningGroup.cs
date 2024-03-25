namespace Massive
{
	public class FullOwningGroup<TFirst, TSecond> : OwningGroup<TFirst, TSecond> where TFirst : struct where TSecond : struct
	{
		public FullOwningGroup(IRegistry registry) : base(registry)
		{
		}

		protected override void SwapEntry(int entryId, int position)
		{
			First.SwapDense(First.GetDense(entryId), position);
			Second.SwapDense(Second.GetDense(entryId), position);
		}
	}
}
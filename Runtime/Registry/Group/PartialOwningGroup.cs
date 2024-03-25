namespace Massive
{
	public class PartialOwningGroup<TOwning, TOther> : OwningGroup<TOwning, TOther>
		where TOwning : struct
		where TOther : struct
	{
		public PartialOwningGroup(IRegistry registry) : base(registry)
		{
		}

		protected override void SwapEntry(int entryId, int position)
		{
			First.SwapDense(First.GetDense(entryId), position);
		}
	}
}
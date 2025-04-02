namespace Massive
{
	public struct SetInfo
	{
		public SparseSet Set;
		public SetCloner Cloner;

		public SetInfo(SparseSet set, SetCloner cloner)
		{
			Set = set;
			Cloner = cloner;
		}
	}
}

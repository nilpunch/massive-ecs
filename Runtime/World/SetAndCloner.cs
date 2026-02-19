namespace Massive
{
	public readonly struct SetAndCloner
	{
		public readonly BitSet Set;
		public readonly SetCloner Cloner;

		public SetAndCloner(BitSet set, SetCloner cloner)
		{
			Set = set;
			Cloner = cloner;
		}

		public void Deconstruct(out BitSet set, out SetCloner cloner)
		{
			set = Set;
			cloner = Cloner;
		}
	}
}

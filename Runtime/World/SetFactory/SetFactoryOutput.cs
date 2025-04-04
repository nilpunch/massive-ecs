namespace Massive
{
	public readonly struct SetFactoryOutput
	{
		public readonly SparseSet Set;
		public readonly SetCloner Cloner;

		public SetFactoryOutput(SparseSet set, SetCloner cloner)
		{
			Set = set;
			Cloner = cloner;
		}

		public void Deconstruct(out SparseSet set, out SetCloner cloner)
		{
			set = Set;
			cloner = Cloner;
		}
	}
}

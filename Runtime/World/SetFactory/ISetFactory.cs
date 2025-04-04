using System;

namespace Massive
{
	public interface ISetFactory
	{
		public readonly struct Output
		{
			public readonly SparseSet Set;
			public readonly SetCloner Cloner;

			public Output(SparseSet set, SetCloner cloner)
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

		Output CreateAppropriateSet<T>() => GenerateVirtualGenericsForAOT<T>();

		private static Output GenerateVirtualGenericsForAOT<T>()
		{
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();

			// Just in case.
			throw new NotSupportedException($"The default implementation of {nameof(CreateAppropriateSet)}<T>() is forbidden.");
		}
	}
}

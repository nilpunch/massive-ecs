using System;

namespace Massive
{
	public interface ISetFactory
	{
		SparseSet CreateAppropriateSet<T>() => GenerateVirtualGenericsForAOT<T>();

		// Hint for AOT
		private static SparseSet GenerateVirtualGenericsForAOT<T>()
		{
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();

			// Just in case 
			throw new NotSupportedException($"The default implementation of {nameof(CreateAppropriateSet)}<T>() is forbidden.");
		}
	}
}

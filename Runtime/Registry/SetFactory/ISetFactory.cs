using System;

namespace Massive
{
	public interface ISetFactory
	{
		ISet CreateAppropriateSet<T>() => GenerateVirtualGenericsForAOT<T>();

		// Hint for AOT
		private static ISet GenerateVirtualGenericsForAOT<T>()
		{
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();

			// Just in case 
			throw new NotSupportedException($"The default implementation of {nameof(CreateAppropriateSet)}<T>() is forbidden.");
		}
	}
}

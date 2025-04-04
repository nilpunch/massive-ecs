using System;

namespace Massive
{
	public interface ISetFactory
	{
		SetFactoryOutput CreateAppropriateSet<T>() => GenerateVirtualGenericsForAOT<T>();

		private static SetFactoryOutput GenerateVirtualGenericsForAOT<T>()
		{
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();

			// Just in case.
			throw new NotSupportedException($"The default implementation of {nameof(CreateAppropriateSet)}<T>() is forbidden.");
		}
	}
}

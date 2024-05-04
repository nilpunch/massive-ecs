using System;

namespace Massive
{
	public interface ISetFactory
	{
		Entities CreateEntities();

		ISet CreateAppropriateSet<T>() => GenerateVirtualGenericsForAOT<T>();

		// This is a hint for IL2CPP
		private static ISet GenerateVirtualGenericsForAOT<T>()
		{
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();

			// Just in case 
			throw new NotSupportedException($"The default implementation of {nameof(CreateAppropriateSet)}<T>() is forbidden.");
		}
	}
}

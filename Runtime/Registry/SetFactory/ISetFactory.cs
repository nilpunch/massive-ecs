using System;

namespace Massive
{
	public interface ISetFactory
	{
		Entities CreateEntities();

		ISet CreateAppropriateSet<T>() => AOT<T>.GenerateVirtualGenerics();

		ISet CreateSparseSet();

		ISet CreateDataSet<T>();

		private static class AOT<T>
		{
			public static ISet GenerateVirtualGenerics()
			{
				// ReSharper disable ReturnValueOfPureMethodIsNotUsed
				new NormalSetFactory().CreateAppropriateSet<T>();
				new MassiveSetFactory().CreateAppropriateSet<T>();

				// Just in case
				throw new NotSupportedException($"The default implementation of {nameof(ISetFactory)}.{nameof(CreateAppropriateSet)}<T>() is forbidden.");
			}
		}
	}
}

using System;

namespace Massive
{
	public interface ISetFactory
	{
		Entities CreateEntities();

		ISet CreateAppropriateSet<T>() => GenerateVirtualGenericsForAOT<T>();

		/// <summary>
		/// This is a hint for IL2CPP to compile
		/// <see cref="CreateAppropriateSet{T}"/> for both <see cref="NormalSetFactory"/> and <see cref="MassiveSetFactory"/>.
		/// Compiler by itself can't figure out which generics need to be compiled.
		/// </summary>
		private static ISet GenerateVirtualGenericsForAOT<T>()
		{
			// ReSharper disable ReturnValueOfPureMethodIsNotUsed
			new NormalSetFactory().CreateAppropriateSet<T>();
			new MassiveSetFactory().CreateAppropriateSet<T>();

			// Just in case 
			throw new NotSupportedException($"The default implementation of {nameof(CreateAppropriateSet)}<T>() is forbidden.");
		}
	}
}

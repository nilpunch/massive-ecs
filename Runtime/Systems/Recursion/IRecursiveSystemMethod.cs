using System;

namespace Massive
{
	public interface IRecursiveSystemMethod<TMethod>
		where TMethod : IRecursiveSystemMethod<TMethod>
	{
		RunResult Run() => throw new NotImplementedException();
	}

	public interface IRecursiveSystemMethod<TMethod, in TArgs>
		where TMethod : IRecursiveSystemMethod<TMethod, TArgs>
	{
		RunResult Run(TArgs args) => throw new NotImplementedException();
	}
}

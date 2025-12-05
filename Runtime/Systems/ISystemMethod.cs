using System;

namespace Massive
{
	public interface ISystemMethod<TMethod> : ISystem
		where TMethod : ISystemMethod<TMethod>
	{
		void Run() => throw new NotImplementedException();
	}

	public interface ISystemMethod<TMethod, in TArgs> : ISystem
		where TMethod : ISystemMethod<TMethod, TArgs>
	{
		void Run(TArgs args) => throw new NotImplementedException();
	}
}

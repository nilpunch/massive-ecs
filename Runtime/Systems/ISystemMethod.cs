using System;

namespace Massive
{
	public interface ISystemMethod<TMethod>
		where TMethod : ISystemMethod<TMethod>
	{
		void Run() => throw new NotImplementedException();
	}

	public interface ISystemMethod<TMethod, in TArgs>
		where TMethod : ISystemMethod<TMethod, TArgs>
	{
		void Run(TArgs args) => throw new NotImplementedException();
	}
}

using System;

namespace Massive
{
	public interface ISystemMethodBase<TMethod> : ISystem
		where TMethod : ISystemMethodBase<TMethod>
	{
	}

	public interface ISystemMethod<TMethod> : ISystemMethodBase<TMethod>
		where TMethod : ISystemMethod<TMethod>
	{
		void Run() => throw new NotImplementedException();
	}

	public interface ISystemMethod<TMethod, in TArgs> : ISystemMethodBase<TMethod>
		where TMethod : ISystemMethod<TMethod, TArgs>
	{
		void Run(TArgs args) => throw new NotImplementedException();
	}
}

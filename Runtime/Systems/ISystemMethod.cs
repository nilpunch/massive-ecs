using System;

namespace Massive
{
	public interface ISystemMethod<TMethod> : ISystem
		where TMethod : ISystemMethod<TMethod>
	{
		void Run() => throw new NotImplementedException();
	}

	public interface ISystemMethod<TMethod, in TArg> : ISystem
		where TMethod : ISystemMethod<TMethod, TArg>
	{
		void Run(TArg arg) => throw new NotImplementedException();
	}

	public interface ISystemMethod<TMethod, in TArg1, in TArg2> : ISystem
		where TMethod : ISystemMethod<TMethod, TArg1, TArg2>
	{
		void Run(TArg1 arg1, TArg2 arg2) => throw new NotImplementedException();
	}

	public interface ISystemMethod<TMethod, in TArg1, in TArg2, in TArg3> : ISystem
		where TMethod : ISystemMethod<TMethod, TArg1, TArg2, TArg3>
	{
		void Run(TArg1 arg1, TArg2 arg2, TArg3 arg3) => throw new NotImplementedException();
	}
}

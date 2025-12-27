using System.Runtime.CompilerServices;

namespace Massive
{
	public interface ISystemInject<TArg> : ISystemMethod<ISystemInject<TArg>, TArg>
	{
		void Inject(TArg arg);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ISystemMethod<ISystemInject<TArg>, TArg>.Run(TArg arg) => Inject(arg);
	}
}

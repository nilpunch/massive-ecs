using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class IncompatibleConfigsException : MassiveException
	{
		private IncompatibleConfigsException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfIncompatible(World a, World b)
		{
			if (!a.Config.CompatibleWith(b.Config))
			{
				throw new IncompatibleConfigsException("Worlds configs are incompatible.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfIncompatible(SetFactory a, SetFactory b)
		{
			if (!a.CompatibleWith(b))
			{
				throw new IncompatibleConfigsException("Factories configs are incompatible.");
			}
		}
	}
}

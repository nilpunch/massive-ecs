using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class RegistryReflectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet Set(this Registry registry, Type type)
		{
			return registry.SetRegistry.Get(type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AssignSet(this Registry registry, Type type, SparseSet set)
		{
			registry.SetRegistry.Assign(type, set);
		}
	}
}

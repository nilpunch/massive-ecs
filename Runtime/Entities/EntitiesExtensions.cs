using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class EntitiesExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entities Clone(this Entities entities)
		{
			var clone = new Entities();
			entities.CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo(this Entities source, Entities destination)
		{
			destination.EnsureCapacityAt(source.MaxId - 1);

			Array.Copy(source.Packed, destination.Packed, source.MaxId);
			Array.Copy(source.Versions, destination.Versions, source.MaxId);
			Array.Copy(source.Sparse, destination.Sparse, source.MaxId);

			destination.CurrentState = source.CurrentState;
		}
	}
}

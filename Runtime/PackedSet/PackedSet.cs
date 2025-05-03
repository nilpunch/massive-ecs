using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public abstract class PackedSet
	{
		/// <summary>
		/// Gets the current number of elements in the packed array.<br/>
		/// May include removed IDs due to holes.
		/// </summary>
		public int Count { get; protected set; }

		/// <summary>
		/// The packed array, containing IDs.<br/>
		/// Don't cache it and use as is, underlying array can be resized at any moment.
		/// </summary>
		public int[] Packed { get; protected set; } = Array.Empty<int>();

		public Packing Packing { get; protected set; }

		/// <summary>
		/// Removes all holes from the packed array.
		/// </summary>
		public abstract void Compact();

		/// <summary>
		/// Changes the current packing, returns previous packing.
		/// </summary>
		public Packing ExchangePacking(Packing packing)
		{
			var previousPacking = Packing;
			if (packing != Packing)
			{
				if (packing == Packing.Continuous)
				{
					Compact();
				}
				Packing = packing;
			}
			return previousPacking;
		}

		/// <summary>
		/// Changes the current packing to a stricter version if the specified packing is stricter,
		/// returns previous packing.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Packing ExchangeToStricterPacking(Packing packing)
		{
			if ((byte)Packing < (byte)packing)
			{
				return ExchangePacking(packing);
			}

			return Packing;
		}
	}
}

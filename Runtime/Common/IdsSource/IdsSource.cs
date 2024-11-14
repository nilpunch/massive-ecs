using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public abstract class IdsSource
	{
		public int Count { get; protected set; }

		public int[] Ids { get; protected set; }

		public Packing Packing { get; protected set; }

		/// <summary>
		/// Changes the current packing, returns previous packing.
		/// </summary>
		public abstract Packing ExchangePacking(Packing packing);

		/// <summary>
		/// Changes the current packing to a stricter version if the specified packing is stricter,
		/// returns previous packing.
		/// </summary>
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

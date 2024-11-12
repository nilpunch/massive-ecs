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
		/// Returns previous packing.
		/// </summary>
		public abstract Packing ExchangePacking(Packing value);
	}
}

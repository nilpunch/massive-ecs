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

		public PackingMode PackingMode { get; protected set; }

		public abstract void ChangePackingMode(PackingMode value);
	}
}

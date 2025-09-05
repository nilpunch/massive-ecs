using System.Linq;

namespace Massive
{
	public interface ISetSelector
	{
		BitSet[] Select(BitSets bitSets);
	}

	public class Selector<T> : ISetSelector
	{
		public BitSet[] Select(BitSets bitSets)
		{
			var result = new BitSet[1];
			result[0] = bitSets.Get<T>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2> : ISetSelector
	{
		public BitSet[] Select(BitSets bitSets)
		{
			var result = new BitSet[2];
			result[0] = bitSets.Get<T1>();
			result[1] = bitSets.Get<T2>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3> : ISetSelector
	{
		public BitSet[] Select(BitSets bitSets)
		{
			var result = new BitSet[3];
			result[0] = bitSets.Get<T1>();
			result[1] = bitSets.Get<T2>();
			result[2] = bitSets.Get<T3>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		public BitSet[] Select(BitSets bitSets)
		{
			return new TSelector().Select(bitSets).Concat(new Selector<T1, T2, T3>().Select(bitSets)).Distinct().ToArray();
		}
	}
}

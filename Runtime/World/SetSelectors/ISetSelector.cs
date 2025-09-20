using System.Linq;

namespace Massive
{
	public interface ISetSelector
	{
		BitSet[] Select(Sets sets);
	}

	public class Selector<T> : ISetSelector
	{
		public BitSet[] Select(Sets sets)
		{
			var result = new BitSet[1];
			result[0] = sets.Get<T>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2> : ISetSelector
	{
		public BitSet[] Select(Sets sets)
		{
			var result = new BitSet[2];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3> : ISetSelector
	{
		public BitSet[] Select(Sets sets)
		{
			var result = new BitSet[3];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			result[2] = sets.Get<T3>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		public BitSet[] Select(Sets sets)
		{
			return new TSelector().Select(sets).Concat(new Selector<T1, T2, T3>().Select(sets)).Distinct().ToArray();
		}
	}
}

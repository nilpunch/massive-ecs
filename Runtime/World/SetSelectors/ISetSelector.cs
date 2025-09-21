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

	public class Selector<T1, T2, T3, T4> : ISetSelector
	{
		public BitSet[] Select(Sets sets)
		{
			var result = new BitSet[4];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			result[2] = sets.Get<T3>();
			result[3] = sets.Get<T4>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, T4, T5> : ISetSelector
	{
		public BitSet[] Select(Sets sets)
		{
			var result = new BitSet[5];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			result[2] = sets.Get<T3>();
			result[3] = sets.Get<T4>();
			result[4] = sets.Get<T5>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, T4, T5, T6> : ISetSelector
	{
		public BitSet[] Select(Sets sets)
		{
			var result = new BitSet[6];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			result[2] = sets.Get<T3>();
			result[3] = sets.Get<T4>();
			result[4] = sets.Get<T5>();
			result[5] = sets.Get<T6>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, T4, T5, T6, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		public BitSet[] Select(Sets sets)
		{
			return new TSelector().Select(sets).Concat(new Selector<T1, T2, T3, T4, T5, T6>().Select(sets)).Distinct().ToArray();
		}
	}
}

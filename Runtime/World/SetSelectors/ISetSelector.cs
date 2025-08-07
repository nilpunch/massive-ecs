using System.Linq;

namespace Massive
{
	public interface ISetSelector
	{
		SparseSet[] Select(Sets sets, bool negative = false);
	}

	public class Selector<T> : ISetSelector
	{
		public SparseSet[] Select(Sets sets, bool negative = false)
		{
			var result = new SparseSet[1];
			result[0] = negative ? sets.Get<Not<T>>() : sets.Get<T>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2> : ISetSelector
	{
		public SparseSet[] Select(Sets sets, bool negative = false)
		{
			var result = new SparseSet[2];
			result[0] = negative ? sets.Get<Not<T1>>() : sets.Get<T1>();
			result[1] = negative ? sets.Get<Not<T2>>() : sets.Get<T2>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3> : ISetSelector
	{
		public SparseSet[] Select(Sets sets, bool negative = false)
		{
			var result = new SparseSet[3];
			result[0] = negative ? sets.Get<Not<T1>>() : sets.Get<T1>();
			result[1] = negative ? sets.Get<Not<T2>>() : sets.Get<T2>();
			result[2] = negative ? sets.Get<Not<T3>>() : sets.Get<T3>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		public SparseSet[] Select(Sets sets, bool negative = false)
		{
			return new TSelector().Select(sets, negative).Concat(new Selector<T1, T2, T3>().Select(sets, negative)).Distinct().ToArray();
		}
	}
}

using System.Linq;

namespace Massive
{
	public interface ISetSelector
	{
		SparseSet[] Select(Sets sets);
	}

	public class Selector<T> : ISetSelector
	{
		public SparseSet[] Select(Sets sets)
		{
			var result = new SparseSet[1];
			result[0] = sets.Get<T>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2> : ISetSelector
	{
		public SparseSet[] Select(Sets sets)
		{
			var result = new SparseSet[2];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3> : ISetSelector
	{
		public SparseSet[] Select(Sets sets)
		{
			var result = new SparseSet[3];
			result[0] = sets.Get<T1>();
			result[1] = sets.Get<T2>();
			result[2] = sets.Get<T3>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		public SparseSet[] Select(Sets sets)
		{
			return new TSelector().Select(sets).Concat(new Selector<T1, T2, T3>().Select(sets)).Distinct().ToArray();
		}
	}
}

﻿using System.Linq;

namespace Massive
{
	public interface ISetSelector
	{
		SparseSet[] Select(SetRegistry setRegistry);
	}

	public class Selector<T> : ISetSelector
	{
		public SparseSet[] Select(SetRegistry setRegistry)
		{
			var result = new SparseSet[1];
			result[0] = setRegistry.Get<T>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2> : ISetSelector
	{
		public SparseSet[] Select(SetRegistry setRegistry)
		{
			var result = new SparseSet[2];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3> : ISetSelector
	{
		public SparseSet[] Select(SetRegistry setRegistry)
		{
			var result = new SparseSet[3];
			result[0] = setRegistry.Get<T1>();
			result[1] = setRegistry.Get<T2>();
			result[2] = setRegistry.Get<T3>();
			return result.Distinct().ToArray();
		}
	}

	public class Selector<T1, T2, T3, TSelector> : ISetSelector
		where TSelector : ISetSelector, new()
	{
		public SparseSet[] Select(SetRegistry setRegistry)
		{
			return new TSelector().Select(setRegistry).Concat(new Selector<T1, T2, T3>().Select(setRegistry)).Distinct().ToArray();
		}
	}
}

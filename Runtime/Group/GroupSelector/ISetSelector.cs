using System.Collections.Generic;

namespace Massive
{
	public interface ISetSelector
	{
		void Select(IRegistry registry, List<ISet> result);
	}

	public interface IReadOnlySetSelector
	{
		void Select(IRegistry registry, List<IReadOnlySet> result);
	}

	public struct None : ISetSelector, IReadOnlySetSelector
	{
		public void Select(IRegistry registry, List<ISet> result)
		{
			result.Clear();
		}

		public void Select(IRegistry registry, List<IReadOnlySet> result)
		{
			result.Clear();
		}
	}

	public struct Many<T> : ISetSelector, IReadOnlySetSelector
	{
		public void Select(IRegistry registry, List<ISet> result)
		{
			result.Clear();
			result.Add(registry.Any<T>());
		}

		public void Select(IRegistry registry, List<IReadOnlySet> result)
		{
			result.Clear();
			result.Add(registry.Any<T>());
		}
	}

	public struct Many<T1, T2> : ISetSelector, IReadOnlySetSelector
	{
		public void Select(IRegistry registry, List<ISet> result)
		{
			result.Clear();
			result.Add(registry.Any<T1>());
			result.Add(registry.Any<T2>());
		}

		public void Select(IRegistry registry, List<IReadOnlySet> result)
		{
			result.Clear();
			result.Add(registry.Any<T1>());
			result.Add(registry.Any<T2>());
		}
	}

	public struct Many<T1, T2, T3> : ISetSelector, IReadOnlySetSelector
	{
		public void Select(IRegistry registry, List<ISet> result)
		{
			result.Clear();
			result.Add(registry.Any<T1>());
			result.Add(registry.Any<T2>());
			result.Add(registry.Any<T3>());
		}

		public void Select(IRegistry registry, List<IReadOnlySet> result)
		{
			result.Clear();
			result.Add(registry.Any<T1>());
			result.Add(registry.Any<T2>());
			result.Add(registry.Any<T3>());
		}
	}
}

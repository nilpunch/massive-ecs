using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class DynamicFilter : Filter
	{
		public Registry Registry { get; }

		public DynamicFilter(Registry registry) : base(Array.Empty<SparseSet>(), Array.Empty<SparseSet>())
		{
			Registry = registry;
		}

		public void Include<T>()
		{
			var set = Registry.Set<T>();

			if (Excluded.Contains(set))
			{
				throw new Exception("Conflict with excluded!");
			}

			if (IncludedCount >= Included.Length)
			{
				var included = Included;
				Array.Resize(ref included, MathUtils.NextPowerOf2(IncludedCount + 1));
				Included = included;
			}

			Included[IncludedCount] = set;
			IncludedCount += 1;
		}

		public void Exclude<T>()
		{
			var set = Registry.Set<T>();

			if (Included.Contains(set))
			{
				throw new Exception("Conflict with included!");
			}

			if (ExcludedCount >= Excluded.Length)
			{
				var included = Excluded;
				Array.Resize(ref included, MathUtils.NextPowerOf2(ExcludedCount + 1));
				Excluded = included;
			}

			Excluded[ExcludedCount] = set;
			ExcludedCount += 1;
		}
	}
}

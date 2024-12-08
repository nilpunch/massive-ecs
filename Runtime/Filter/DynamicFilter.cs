﻿using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class DynamicFilter : Filter
	{
		public SetRegistry SetRegistry { get; }

		public DynamicFilter(Registry registry) : base(Array.Empty<SparseSet>(), Array.Empty<SparseSet>())
		{
			SetRegistry = registry.SetRegistry;
		}

		public DynamicFilter(SetRegistry setRegistry) : base(Array.Empty<SparseSet>(), Array.Empty<SparseSet>())
		{
			SetRegistry = setRegistry;
		}

		public DynamicFilter Include<T>()
		{
			var set = SetRegistry.Get<T>();

			if (Excluded.Contains(set))
			{
				throw new Exception("Conflict with excluded!");
			}

			if (Included.Contains(set))
			{
				return this;
			}

			if (IncludedCount >= Included.Length)
			{
				var included = Included;
				Array.Resize(ref included, MathUtils.NextPowerOf2(IncludedCount + 1));
				Included = included;
			}

			Included[IncludedCount] = set;
			IncludedCount += 1;

			return this;
		}

		public DynamicFilter Exclude<T>()
		{
			var set = SetRegistry.Get<T>();

			if (Included.Contains(set))
			{
				throw new Exception("Conflict with included!");
			}

			if (Excluded.Contains(set))
			{
				return this;
			}

			if (ExcludedCount >= Excluded.Length)
			{
				var included = Excluded;
				Array.Resize(ref included, MathUtils.NextPowerOf2(ExcludedCount + 1));
				Excluded = included;
			}

			Excluded[ExcludedCount] = set;
			ExcludedCount += 1;

			return this;
		}
	}
}
﻿using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public sealed class SparseSetCloner<T> : SetCloner
	{
		private readonly SparseSet _sparseSet;

		public SparseSetCloner(SparseSet sparseSet)
		{
			_sparseSet = sparseSet;
		}

		public override void CopyTo(SetRegistry setRegistry)
		{
			_sparseSet.CopySparseTo(setRegistry.Get<T>());
		}
	}
}

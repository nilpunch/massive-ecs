using System;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class Filter
	{
		public static Filter Default { get; } = new Filter();

		private readonly ArraySegment<IReadOnlySet> _mustInclude;
		private readonly ArraySegment<IReadOnlySet> _mustExclude;

		public Filter(IReadOnlySet[] include = null, IReadOnlySet[] exclude = null)
		{
			_mustInclude = include ?? ArraySegment<IReadOnlySet>.Empty;
			_mustExclude = exclude ?? ArraySegment<IReadOnlySet>.Empty;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsOkay(int id)
		{
			if (_mustExclude.Count == 0 && _mustInclude.Count == 0)
			{
				return true;
			}

			foreach (var exclude in _mustExclude)
			{
				if (exclude.IsAlive(id))
				{
					return false;
				}
			}

			foreach (var include in _mustInclude)
			{
				if (!include.IsAlive(id))
				{
					return false;
				}
			}

			return true;
		}
	}
}
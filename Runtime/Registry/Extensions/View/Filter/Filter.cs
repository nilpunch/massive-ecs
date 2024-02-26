using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive.ECS
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Filter
	{
		private readonly IRegistry _registry;
		private readonly List<IReadOnlySet> _mustInclude = new List<IReadOnlySet>();
		private readonly List<IReadOnlySet> _mustExclude = new List<IReadOnlySet>();

		public Filter(IRegistry registry)
		{
			_registry = registry;
		}

		public void Include<T>() where T : unmanaged
		{
			_mustInclude.Add(_registry.AnySet<T>());
		}

		public void Exclude<T>() where T : unmanaged
		{
			_mustExclude.Add(_registry.AnySet<T>());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsOkay(int id)
		{
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class Filter
	{
		private readonly Registry _registry;
		private readonly List<IReadOnlySet> _mustInclude = new List<IReadOnlySet>();
		private readonly List<IReadOnlySet> _mustExclude = new List<IReadOnlySet>();

		public Filter(Registry registry)
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
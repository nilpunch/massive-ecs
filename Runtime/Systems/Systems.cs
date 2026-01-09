using System;
using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class Systems
	{
		private Array[] _systemsLookup = Array.Empty<Array>();
		private ISystem[] _systems = Array.Empty<ISystem>();

		public ISystem[] GetAllSystems()
		{
			return _systems;
		}

		public Systems Run<TSystemMethod>()
			where TSystemMethod : ISystemMethod<TSystemMethod>
		{
			foreach (var method in GetSystemsOfType<TSystemMethod>())
			{
				method.Run();
			}

			return this;
		}

		public Systems Run<TSystemMethod, TArgs>(TArgs args)
			where TSystemMethod : ISystemMethod<TSystemMethod, TArgs>
		{
			foreach (var method in GetSystemsOfType<TSystemMethod>())
			{
				method.Run(args);
			}

			return this;
		}

		public TSystemMethod[] GetSystemsOfType<TSystemMethod>()
		{
			var lookupIndex = TypeId<SystemMethodKind, TSystemMethod>.Info.Index;

			if (lookupIndex >= _systemsLookup.Length)
			{
				_systemsLookup = _systemsLookup.ResizeToNextPowOf2(lookupIndex + 1);
			}

			var candidate = _systemsLookup[lookupIndex];

			if (candidate != null)
			{
				return (TSystemMethod[])candidate;
			}

			var systemMethods = new List<TSystemMethod>();
			foreach (var system in _systems)
			{
				if (system is TSystemMethod runMethod)
				{
					systemMethods.Add(runMethod);
				}
			}
			var systemsArray = systemMethods.ToArray();
			_systemsLookup[lookupIndex] = systemsArray;
			return systemsArray;
		}
	}

	internal struct SystemMethodKind
	{
	}
}

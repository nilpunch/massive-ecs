using System;
using System.Collections.Generic;

namespace Massive
{
	internal struct SystemMethodKind
	{
	}

	public partial class Systems
	{
		private Array[] _systemsLookup = Array.Empty<Array>();
		private ISystem[] _systems = Array.Empty<ISystem>();

		public void Run<TSystemMethod>()
			where TSystemMethod : ISystemMethod<TSystemMethod>
		{
			foreach (var method in GetSystemsOfType<TSystemMethod>())
			{
				method.Run();
			}
		}

		public void Run<TSystemMethod, TArgs>(TArgs args)
			where TSystemMethod : ISystemMethod<TSystemMethod, TArgs>
		{
			foreach (var method in GetSystemsOfType<TSystemMethod>())
			{
				method.Run(args);
			}
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
}

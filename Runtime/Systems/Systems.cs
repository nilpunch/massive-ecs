using System;
using System.Collections.Generic;

namespace Massive
{
	public partial class Systems
	{
		public class SystemsCache<TSystemMethod>
			where TSystemMethod : ISystemMethodBase<TSystemMethod>
		{
			public TSystemMethod[] SystemMethods;
			public TSystemMethod[] RecursiveSystemMethods;
			public IRecursive<TSystemMethod>[] RecursiveConditionsArray;
		}

		private readonly Dictionary<Type, object> _systemsCache = new Dictionary<Type, object>();
		private ISystem[] _systems = Array.Empty<ISystem>();

		public void Run<TSystemMethod>()
			where TSystemMethod : ISystemMethod<TSystemMethod>
		{
			var systemsCache = GetSystemsCache<TSystemMethod>();
			foreach (var method in systemsCache.SystemMethods)
			{
				method.Run();
			}

			bool running;
			do
			{
				running = false;
				for (var i = 0; i < systemsCache.RecursiveConditionsArray.Length; i++)
				{
					if (systemsCache.RecursiveConditionsArray[i].NeedRerun)
					{
						systemsCache.RecursiveSystemMethods[i].Run();
						running = true;
					}
				}
			} while (running);
		}

		public void Run<TSystemMethod, TArgs>(TArgs args)
			where TSystemMethod : ISystemMethod<TSystemMethod, TArgs>
		{
			var systemsCache = GetSystemsCache<TSystemMethod>();
			foreach (var method in systemsCache.SystemMethods)
			{
				method.Run(args);
			}

			bool running;
			do
			{
				running = false;
				for (var i = 0; i < systemsCache.RecursiveConditionsArray.Length; i++)
				{
					if (systemsCache.RecursiveConditionsArray[i].NeedRerun)
					{
						systemsCache.RecursiveSystemMethods[i].Run(args);
						running = true;
					}
				}
			} while (running);
		}

		public TSystemMethod[] GetSystems<TSystemMethod>()
			where TSystemMethod : ISystemMethodBase<TSystemMethod>
		{
			return GetSystemsCache<TSystemMethod>().SystemMethods;
		}

		public SystemsCache<TSystemMethod> GetSystemsCache<TSystemMethod>()
			where TSystemMethod : ISystemMethodBase<TSystemMethod>
		{
			var type = typeof(TSystemMethod);

			if (_systemsCache.TryGetValue(type, out var systemsCache))
			{
				return (SystemsCache<TSystemMethod>)systemsCache;
			}

			var systemMethods = new List<TSystemMethod>();
			var recursiveSystemMethods = new List<TSystemMethod>();
			var conditions = new List<IRecursive<TSystemMethod>>();
			foreach (var system in _systems)
			{
				if (system is not TSystemMethod runMethod)
				{
					continue;
				}

				systemMethods.Add(runMethod);

				if (system is IRecursive<TSystemMethod> condition)
				{
					recursiveSystemMethods.Add(runMethod);
					conditions.Add(condition);
				}
			}
			systemsCache = new SystemsCache<TSystemMethod>
			{
				SystemMethods = systemMethods.ToArray(),
				RecursiveSystemMethods = recursiveSystemMethods.ToArray(),
				RecursiveConditionsArray = conditions.ToArray(),
			};
			_systemsCache[type] = systemsCache;
			return (SystemsCache<TSystemMethod>)systemsCache;
		}
	}
}

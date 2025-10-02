using System;
using System.Collections;
using System.Collections.Generic;

namespace Massive
{
	public class Systems
	{
		private readonly List<ISystem> _systems = new List<ISystem>();
		private readonly Dictionary<Type, IList> _systemsCache = new Dictionary<Type, IList>();

		public World World { get; }

		public Systems(World world)
		{
			World = world;
		}

		public void AddSystem(ISystem system)
		{
			system.World = World;
			_systems.Add(system);

			// Cache is invalid after modifying systems.
			_systemsCache.Clear();
		}

		public void Run<TSystemMethod>()
			where TSystemMethod : ISystemMethod<TSystemMethod>
		{
			foreach (var method in GetMethods<TSystemMethod>())
			{
				method.Run();
			}
		}

		public void Run<TSystemMethod, TArg>(TArg arg)
			where TSystemMethod : ISystemMethod<TSystemMethod, TArg>
		{
			foreach (var method in GetMethods<TSystemMethod>())
			{
				method.Run(arg);
			}
		}

		public void Run<TSystemMethod, TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
			where TSystemMethod : ISystemMethod<TSystemMethod, TArg1, TArg2>
		{
			foreach (var method in GetMethods<TSystemMethod>())
			{
				method.Run(arg1, arg2);
			}
		}

		public void Run<TSystemMethod, TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
			where TSystemMethod : ISystemMethod<TSystemMethod, TArg1, TArg2, TArg3>
		{
			foreach (var method in GetMethods<TSystemMethod>())
			{
				method.Run(arg1, arg2, arg3);
			}
		}

		private List<TSystemMethod> GetMethods<TSystemMethod>()
		{
			var type = typeof(TSystemMethod);

			if (_systemsCache.TryGetValue(type, out var runMethodsList))
			{
				return (List<TSystemMethod>)runMethodsList;
			}

			var systemMethods = new List<TSystemMethod>();
			foreach (var system in _systems)
			{
				if (system is TSystemMethod runMethod)
				{
					systemMethods.Add(runMethod);
				}
			}
			_systemsCache[type] = systemMethods;
			return systemMethods;
		}
	}
}

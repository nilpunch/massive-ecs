using System;
using System.Collections.Generic;

namespace Massive
{
	public partial class Systems
	{
		private readonly Dictionary<Type, Array> _systemsCache = new Dictionary<Type, Array>();
		private ISystem[] _systems = Array.Empty<ISystem>();

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

		private TSystemMethod[] GetMethods<TSystemMethod>()
		{
			var type = typeof(TSystemMethod);

			if (_systemsCache.TryGetValue(type, out var runMethodsList))
			{
				return (TSystemMethod[])runMethodsList;
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
			_systemsCache[type] = systemsArray;
			return systemsArray;
		}
	}
}

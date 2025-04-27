using System;
using System.Collections.Generic;
using System.Reflection;

namespace Massive
{
	public static class DefaultValueUtils
	{
		private static readonly Dictionary<Type, object> _defaultValuesCache = new Dictionary<Type, object>();

		public static T GetDefaultValueFor<T>()
		{
			var type = typeof(T);

			if (_defaultValuesCache.TryGetValue(type, out var cached))
			{
				return (T)cached;
			}

			var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			foreach (var property in type.GetProperties(flags))
			{
				if (property.IsDefined(typeof(DefaultValueAttribute), inherit: false) &&
					property.PropertyType == type &&
					property.GetMethod != null)
				{
					var value = (T)property.GetValue(null);
					_defaultValuesCache[type] = value;
					return value;
				}
			}

			_defaultValuesCache[type] = default(T);
			return default;
		}
	}
}

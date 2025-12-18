using System.Reflection;

namespace Massive
{
	public static class Default<T>
	{
		public static readonly T Value;

		static Default()
		{
			var type = typeof(T);

			var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			foreach (var property in type.GetProperties(flags))
			{
				if (property.IsDefined(typeof(DefaultValueAttribute), inherit: false) &&
					property.PropertyType == type &&
					property.GetMethod != null)
				{
					var value = (T)property.GetValue(null);
					Value = value;
				}
			}

			Value = default;
		}
	}
}

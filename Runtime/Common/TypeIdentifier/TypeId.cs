using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

// ReSharper disable all StaticMemberInGenericType
namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public abstract class TypeId<T>
	{
		private static bool s_initialized;

		public static TypeIdInfo Info;

		static TypeId()
		{
			Warmup();
		}

		public static void Warmup()
		{
			if (s_initialized)
			{
				return;
			}

			var type = typeof(T);
			var index = TypesCounter.Increment();
			var typeName = type.GetFullGenericName();

			var info = new TypeIdInfo(index, typeName);

			Info = info;
			s_initialized = true;

			RuntimeTypeId.Register(type, info);
		}
	}
}

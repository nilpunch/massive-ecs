using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public abstract class TypeIdentifier<T>
	{
		private static IdentifierInfo s_info;
		private static bool s_initialized;

		public static IdentifierInfo Info
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => s_info;
		}

		static TypeIdentifier()
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

			var info = new IdentifierInfo(index, typeName);

			s_info = info;
			s_initialized = true;

			RuntimeTypeIdentifier.Register(type, info);
		}
	}
}

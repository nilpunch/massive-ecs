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
		public static readonly TypeIdInfo Info;

		static TypeId()
		{
			var type = typeof(T);
			var index = TypesCounter.Increment();
			var typeName = type.GetFullGenericName();

			var info = new TypeIdInfo(index, typeName);

			Info = info;

			RuntimeTypeId.Register(type, info);
		}
	}
}

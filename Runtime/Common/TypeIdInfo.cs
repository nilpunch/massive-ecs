using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct TypeIdInfo
	{
		/// <summary>
		/// Session-dependent index, used for lookups.<br/>
		/// </summary>
		public readonly int Index;

		public readonly string FullName;

		public readonly Type Type;

		public TypeIdInfo(int index, string fullName, Type type)
		{
			Index = index;
			FullName = fullName;
			Type = type;
		}
	}
}

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Unity.IL2CPP.CompilerServices
{
	using System;

	[Flags]
	internal enum Option
	{
		NullChecks = 1,
		ArrayBoundsChecks = 2,
		DivideByZeroChecks = 3
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	internal class Il2CppSetOptionAttribute : Attribute
	{
		public Option Option { get; }
		public object Value { get; }

		public Il2CppSetOptionAttribute(Option option, object value)
		{
			Option = option;
			Value = value;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	internal class Il2CppEagerStaticClassConstructionAttribute : Attribute
	{
	}
}

namespace UnityEngine.Scripting
{
	using System;

	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct, Inherited = false)]
	public class PreserveAttribute : Attribute
	{
	}
}

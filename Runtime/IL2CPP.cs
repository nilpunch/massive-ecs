namespace Unity.IL2CPP.CompilerServices
{
	using System;

#if !ENABLE_IL2CPP
	public enum Option
	{
#else
	internal enum Option
	{
#endif
		NullChecks = 1,
		ArrayBoundsChecks = 2,
		DivideByZeroChecks = 3
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
#if !ENABLE_IL2CPP
	public class Il2CppSetOptionAttribute : Attribute
	{
#else
	internal class Il2CppSetOptionAttribute : Attribute
	{
#endif
		public Option Option { get; }
		public object Value { get; }

		public Il2CppSetOptionAttribute(Option option, object value)
		{
			Option = option;
			Value = value;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
#if !ENABLE_IL2CPP
	public class Il2CppEagerStaticClassConstructionAttribute : Attribute
	{
#else
	internal class Il2CppEagerStaticClassConstructionAttribute : Attribute
	{
#endif
	}
}
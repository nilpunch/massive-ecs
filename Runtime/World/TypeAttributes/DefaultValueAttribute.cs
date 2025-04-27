using System;

namespace Massive
{
	/// <summary>
	/// Marks this property as default value for this type.
	/// Will be used only with unmanaged components.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultValueAttribute : Attribute
	{
	}
}

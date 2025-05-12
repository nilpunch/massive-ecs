using System;

namespace Massive
{
	/// <summary>
	/// Marks this property as default value for this type.<br/>
	/// Works only with unmanaged components and allocator.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultValueAttribute : Attribute
	{
	}
}

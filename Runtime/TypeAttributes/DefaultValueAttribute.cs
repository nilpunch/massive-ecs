using System;

namespace Massive
{
	/// <summary>
	/// Marks this static property as default value for this type.<br/>
	/// Works with components and allocator data.<br/>
	/// The default value is initialized once and shared.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultValueAttribute : Attribute
	{
	}
}

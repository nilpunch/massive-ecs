using System;

namespace Massive
{
	/// <summary>
	/// Marks this static property as default value for this type.<br/>
	/// Works with components, allocator data, and inputs.<br/>
	/// The default value is initialized once and shared.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultValueAttribute : Attribute
	{
	}
}

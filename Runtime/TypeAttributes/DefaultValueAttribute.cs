using System;

namespace Massive
{
	/// <summary>
	/// Marks this static property as default value for this type.<br/>
	/// Works only with unmanaged components and allocator data.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultValueAttribute : Attribute
	{
	}
}

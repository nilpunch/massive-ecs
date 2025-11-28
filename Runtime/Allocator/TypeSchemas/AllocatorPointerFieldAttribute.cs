using System;

namespace Massive
{
	[AttributeUsage(AttributeTargets.Field)]
	public class AllocatorPointerFieldAttribute : Attribute
	{
		public string CountFieldName { get; set; } = string.Empty;
	}
}

using System;

namespace Massive
{
	[AttributeUsage(AttributeTargets.Field)]
	public class PointerFieldAttribute : Attribute
	{
		public string CountFieldName { get; set; } = string.Empty;
	}
}

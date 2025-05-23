using System;

namespace Massive
{
	/// <summary>
	/// Overrides page size for the data set of this component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate)]
	public class PageSizeAttribute : Attribute
	{
		public int PageSize { get; }

		/// <param name="pageSize">
		/// Must be power of two and not 0.
		/// </param>
		public PageSizeAttribute(int pageSize)
		{
			PageSize = pageSize;
		}
	}
}

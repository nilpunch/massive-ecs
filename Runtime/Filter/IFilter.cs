using System.Collections.Generic;

namespace Massive
{
	public interface IFilter
	{
		IReadOnlyList<IReadOnlySet> Include { get; }
		IReadOnlyList<IReadOnlySet> Exclude { get; }

		bool ContainsId(int id);
	}
}

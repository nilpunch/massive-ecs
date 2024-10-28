using System;

namespace Massive
{
	public interface IFilter
	{
		SparseSet[] Include { get; }

		bool ContainsId(int id);
	}
}

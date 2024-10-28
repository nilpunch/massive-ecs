using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class IncludeFilter : IFilter
	{
		private readonly SparseSet[] _include;

		public IncludeFilter(SparseSet[] include)
		{
			_include = include;
		}

		public SparseSet[] Include => _include;

		public bool ContainsId(int id)
		{
			return id >= 0 && SetHelpers.AssignedInAll(id, _include);
		}
	}
}

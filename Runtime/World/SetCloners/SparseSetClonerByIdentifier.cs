using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class SparseSetClonerByIdentifier : SetCloner
	{
		private readonly SparseSet _sparseSet;
		private readonly string _setId;

		public SparseSetClonerByIdentifier(SparseSet sparseSet, string setId)
		{
			_sparseSet = sparseSet;
			_setId = setId;
		}

		public override void CopyTo(Sets sets)
		{
			var destination = sets.GetExisting(_setId);

			if (destination == null)
			{
				var clone = _sparseSet.CloneSparse();
				sets.Insert(_setId, clone, new SparseSetClonerByIdentifier(clone, _setId));
			}
			else
			{
				_sparseSet.CopySparseTo(destination);
			}
		}
	}
}

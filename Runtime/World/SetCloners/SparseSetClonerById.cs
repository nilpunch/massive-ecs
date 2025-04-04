using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public sealed class SparseSetClonerById : SetCloner
	{
		private readonly SparseSet _sparseSet;
		private readonly string _setId;

		public SparseSetClonerById(SparseSet sparseSet, string setId)
		{
			_sparseSet = sparseSet;
			_setId = setId;
		}

		public override void CopyTo(SetRegistry setRegistry)
		{
			var destination = setRegistry.GetExisting(_setId);

			if (destination == null)
			{
				var clone = _sparseSet.CloneSparse();
				setRegistry.Insert(_setId, clone, new SparseSetClonerById(clone, _setId));
			}
			else
			{
				_sparseSet.CopySparseTo(destination);
			}
		}
	}
}
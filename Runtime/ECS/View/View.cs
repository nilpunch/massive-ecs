using System.Runtime.CompilerServices;

namespace Massive.ECS
{
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public readonly struct View
	{
		private readonly ISet _tags;

		public View(ISet tags)
		{
			_tags = tags;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var ids = _tags.AliveIds;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(ids[dense]);
			}
		}
	}
}
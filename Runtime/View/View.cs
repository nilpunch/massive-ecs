using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View : IView
	{
		private readonly IEntities _entities;

		public View(IRegistry registry)
		{
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker
		{
			var entities = _entities.Alive;
			for (var i = entities.Length - 1; i >= 0; i--)
			{
				invoker.Apply(entities[i].Id);
			}
		}
	}
}

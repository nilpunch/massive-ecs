using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class SetRegistry : TypeRegistry<ISet>
	{
		private readonly ISetFactory _setFactory;

		public SetRegistry(ISetFactory setFactory)
		{
			_setFactory = setFactory;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet Get<TKey>()
		{
			var set = GetOrNull<TKey>();

			if (set == null)
			{
				set = _setFactory.CreateAppropriateSet<TKey>();
				Bind<TKey>(set);
			}

			return set;
		}
	}
}

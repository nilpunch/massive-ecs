using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class ServiceLocator
	{
		private readonly GenericLookup<object> _lookup = new GenericLookup<object>();

		public FastList<object> All
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _lookup.All;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign<TItem>(TItem item)
		{
			_lookup.Assign<TItem>(item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(Type type, object item)
		{
			_lookup.Assign(type, item);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TItem Find<TItem>()
		{
			return (TItem)_lookup.Find<TItem>();
		}
	}
}

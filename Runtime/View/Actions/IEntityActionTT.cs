﻿using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2>
	{
		bool Apply(int id, ref T1 a, ref T2 b);
	}

	public struct EntityActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public EntityActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b);
			return true;
		}
	}

	public struct EntityActionRefExtraAdapter<T1, T2, TExtra> : IEntityAction<T1, T2>
	{
		public EntityActionRefExtra<T1, T2, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b, Extra);
			return true;
		}
	}

	public struct ActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public ActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b);
			return true;
		}
	}

	public struct ActionRefExtraAdapter<T1, T2, TExtra> : IEntityAction<T1, T2>
	{
		public ActionRefExtra<T1, T2, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b, Extra);
			return true;
		}
	}
}

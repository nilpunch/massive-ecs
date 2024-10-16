using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T>
	{
		bool Apply(int id, ref T a);
	}

	public struct EntityActionRefAdapter<T> : IEntityAction<T>
	{
		public EntityActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a);
			return true;
		}
	}

	public struct EntityActionRefExtraAdapter<T, TExtra> : IEntityAction<T>
	{
		public EntityActionRefExtra<T, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a, Extra);
			return true;
		}
	}

	public struct ActionRefAdapter<T> : IEntityAction<T>
	{
		public ActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(ref a);
			return true;
		}
	}

	public struct ActionRefExtraAdapter<T, TExtra> : IEntityAction<T>
	{
		public ActionRefExtra<T, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(ref a, Extra);
			return true;
		}
	}
}

using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T>
	{
		void Apply(int id, ref T a);
	}

	public struct EntityActionRefAdapter<T> : IEntityAction<T>
	{
		public EntityActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a);
		}
	}

	public struct EntityActionRefExtraAdapter<T, TExtra> : IEntityAction<T>
	{
		public EntityActionRefExtra<T, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a, Extra);
		}
	}

	public struct ActionRefAdapter<T> : IEntityAction<T>
	{
		public ActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(ref a);
		}
	}

	public struct ActionRefExtraAdapter<T, TExtra> : IEntityAction<T>
	{
		public ActionRefExtra<T, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(ref a, Extra);
		}
	}
}

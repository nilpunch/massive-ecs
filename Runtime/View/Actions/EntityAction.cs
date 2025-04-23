namespace Massive
{
	public delegate void EntityAction(int id);

	public delegate void EntityActionRef<T>(int id, ref T a);

	public delegate void EntityActionRef<T1, T2>(int id, ref T1 a, ref T2 b);

	public delegate void EntityActionRef<T1, T2, T3>(int id, ref T1 a, ref T2 b, ref T3 c);

	public delegate void EntityActionRef<T1, T2, T3, T4>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d);

	public delegate void EntityActionArgs<TArgs>(int id, TArgs args);

	public delegate void EntityActionRefArgs<T, TArgs>(int id, ref T a, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, TArgs>(int id, ref T1 a, ref T2 b, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, T3, TArgs>(int id, ref T1 a, ref T2 b, ref T3 c, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, T3, T4, TArgs>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d, TArgs args);

	public delegate void ActionRef<T>(ref T a);

	public delegate void ActionRef<T1, T2>(ref T1 a, ref T2 b);

	public delegate void ActionRef<T1, T2, T3>(ref T1 a, ref T2 b, ref T3 c);

	public delegate void ActionRef<T1, T2, T3, T4>(ref T1 a, ref T2 b, ref T3 c, ref T4 d);

	public delegate void ActionRefArgs<T, TArgs>(ref T a, TArgs args);

	public delegate void ActionRefArgs<T1, T2, TArgs>(ref T1 a, ref T2 b, TArgs args);

	public delegate void ActionRefArgs<T1, T2, T3, TArgs>(ref T1 a, ref T2 b, ref T3 c, TArgs args);

	public delegate void ActionRefArgs<T1, T2, T3, T4, TArgs>(ref T1 a, ref T2 b, ref T3 c, ref T4 d, TArgs args);
}

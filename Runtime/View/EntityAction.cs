namespace Massive
{
	public delegate void EntityAction(int id);

	public delegate void EntityActionRef<T>(int id, ref T a);

	public delegate void EntityActionRef<T1, T2>(int id, ref T1 a, ref T2 b);

	public delegate void EntityActionRef<T1, T2, T3>(int id, ref T1 a, ref T2 b, ref T3 c);

	public delegate void EntityActionExtra<TExtra>(int id, TExtra extra);

	public delegate void EntityActionRefExtra<T, TExtra>(int id, ref T a, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, TExtra>(int id, ref T1 a, ref T2 b, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, T3, TExtra>(int id, ref T1 a, ref T2 b, ref T3 c, TExtra extra);

	public delegate void ActionRef<T>(ref T a);

	public delegate void ActionRef<T1, T2>(ref T1 a, ref T2 b);

	public delegate void ActionRef<T1, T2, T3>(ref T1 a, ref T2 b, ref T3 c);

	public delegate void ActionRefExtra<T, TExtra>(ref T a, TExtra extra);

	public delegate void ActionRefExtra<T1, T2, TExtra>(ref T1 a, ref T2 b, TExtra extra);

	public delegate void ActionRefExtra<T1, T2, T3, TExtra>(ref T1 a, ref T2 b, ref T3 c, TExtra extra);
}

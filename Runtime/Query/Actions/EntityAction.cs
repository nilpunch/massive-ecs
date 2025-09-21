namespace Massive
{
	public delegate void IdAction(int id);

	public delegate void IdActionRef<T>(int id, ref T a);

	public delegate void IdActionRef<T1, T2>(int id, ref T1 a, ref T2 b);

	public delegate void IdActionRef<T1, T2, T3>(int id, ref T1 a, ref T2 b, ref T3 c);

	public delegate void IdActionRef<T1, T2, T3, T4>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d);

	public delegate void IdActionRef<T1, T2, T3, T4, T5>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e);

	public delegate void IdActionRef<T1, T2, T3, T4, T5, T6>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, ref T6 f);

	public delegate void IdActionArgs<TArgs>(int id, TArgs args);

	public delegate void IdActionRefArgs<T, TArgs>(int id, ref T a, TArgs args);

	public delegate void IdActionRefArgs<T1, T2, TArgs>(int id, ref T1 a, ref T2 b, TArgs args);

	public delegate void IdActionRefArgs<T1, T2, T3, TArgs>(int id, ref T1 a, ref T2 b, ref T3 c, TArgs args);

	public delegate void IdActionRefArgs<T1, T2, T3, T4, TArgs>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d, TArgs args);

	public delegate void IdActionRefArgs<T1, T2, T3, T4, T5, TArgs>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, TArgs args);

	public delegate void IdActionRefArgs<T1, T2, T3, T4, T5, T6, TArgs>(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, ref T6 f, TArgs args);

	public delegate void EntityAction(Entity entity);

	public delegate void EntityActionRef<T>(Entity entity, ref T a);

	public delegate void EntityActionRef<T1, T2>(Entity entity, ref T1 a, ref T2 b);

	public delegate void EntityActionRef<T1, T2, T3>(Entity entity, ref T1 a, ref T2 b, ref T3 c);

	public delegate void EntityActionRef<T1, T2, T3, T4>(Entity entity, ref T1 a, ref T2 b, ref T3 c, ref T4 d);

	public delegate void EntityActionRef<T1, T2, T3, T4, T5>(Entity entity, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e);

	public delegate void EntityActionRef<T1, T2, T3, T4, T5, T6>(Entity entity, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, ref T6 f);

	public delegate void EntityActionArgs<TArgs>(Entity entity, TArgs args);

	public delegate void EntityActionRefArgs<T, TArgs>(Entity entity, ref T a, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, TArgs>(Entity entity, ref T1 a, ref T2 b, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, T3, TArgs>(Entity entity, ref T1 a, ref T2 b, ref T3 c, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, T3, T4, TArgs>(Entity entity, ref T1 a, ref T2 b, ref T3 c, ref T4 d, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, T3, T4, T5, TArgs>(Entity entity, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, TArgs args);

	public delegate void EntityActionRefArgs<T1, T2, T3, T4, T5, T6, TArgs>(Entity entity, ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, ref T6 f, TArgs args);

	public delegate void ActionRef<T>(ref T a);

	public delegate void ActionRef<T1, T2>(ref T1 a, ref T2 b);

	public delegate void ActionRef<T1, T2, T3>(ref T1 a, ref T2 b, ref T3 c);

	public delegate void ActionRef<T1, T2, T3, T4>(ref T1 a, ref T2 b, ref T3 c, ref T4 d);

	public delegate void ActionRef<T1, T2, T3, T4, T5>(ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e);

	public delegate void ActionRef<T1, T2, T3, T4, T5, T6>(ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, ref T6 f);

	public delegate void ActionRefArgs<T, TArgs>(ref T a, TArgs args);

	public delegate void ActionRefArgs<T1, T2, TArgs>(ref T1 a, ref T2 b, TArgs args);

	public delegate void ActionRefArgs<T1, T2, T3, TArgs>(ref T1 a, ref T2 b, ref T3 c, TArgs args);

	public delegate void ActionRefArgs<T1, T2, T3, T4, TArgs>(ref T1 a, ref T2 b, ref T3 c, ref T4 d, TArgs args);

	public delegate void ActionRefArgs<T1, T2, T3, T4, T5, TArgs>(ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, TArgs args);

	public delegate void ActionRefArgs<T1, T2, T3, T4, T5, T6, TArgs>(ref T1 a, ref T2 b, ref T3 c, ref T4 d, ref T5 e, ref T6 f, TArgs args);
}

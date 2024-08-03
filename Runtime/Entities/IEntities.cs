using System;
using System.Diagnostics.CodeAnalysis;

namespace Massive
{
	public interface IEntities
	{
		int Count { get; }

		ReadOnlySpan<Entity> Alive { get; }

		/// <summary>
		/// Shoots after <see cref="IEntities.Create"/> call.
		/// </summary>
		event Action<Entity> AfterCreated;

		/// <summary>
		/// Shoots before each <see cref="IEntities.Destroy"/> call, when the id was alive.
		/// </summary>
		event Action<int> BeforeDestroyed;

		Entity Create();

		void Destroy(int id);

		void CreateMany(int amount, [MaybeNull] Action<Entity> action = null);

		Entity GetEntity(int id);

		bool IsAlive(Entity entity);

		bool IsAlive(int id);

		void ResizeDense(int capacity);

		void ResizeSparse(int capacity);
	}
}

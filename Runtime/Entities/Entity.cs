using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Feature-rich entity hanle.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial struct Entity : IEquatable<Entifier>, IEquatable<Entity>
	{
		public int Id;

		public uint Version;

		public World World;

		public Entity(Entifier entifier, World world)
		{
			Id = entifier.Id;
			Version = entifier.Version;
			World = world;
		}

		public Entity(int id, uint version, World world)
		{
			Id = id;
			Version = version;
			World = world;
		}

		public static Entity Dead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entity(0, 0, null);
		}

		/// <summary>
		/// Checks whether the entity is alive.
		/// </summary>
		public readonly bool IsAlive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => World != null && World.Entities.IsAlive(Entifier);
		}

		public readonly Entifier Entifier
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entifier(Id, Version);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Entity a, Entifier b)
		{
			return a.Equals(b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Entifier b, Entity a)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Entifier b, Entity a)
		{
			return !(b == a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Entity a, Entifier b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Entity a, Entity b)
		{
			return a.Equals(b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Entity a, Entity b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Entifier other)
		{
			return Id == other.Id && Version == other.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Entity other)
		{
			return Id == other.Id && Version == other.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override bool Equals(object obj)
		{
			return obj is Entifier other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override int GetHashCode()
		{
			return MathUtils.CombineHashes(Id, (int)Version);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly override string ToString()
		{
			return $"(id:{Id} v:{Version})";
		}
	}
}

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly partial struct Entity : IEquatable<Entifier>, IEquatable<Entity>
	{
		/// <summary>
		/// 0 counted as invalid and dead.<br/>
		/// [ Version: 32 bits | ID: 32 bits ]
		/// </summary>
		public readonly long VersionAndId;

		public readonly World World;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)VersionAndId;
		}

		/// <summary>
		/// Entities with version 0 are invalid and counted as dead.
		/// </summary>
		public uint Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)(VersionAndId >> 32);
		}

		public Entity(Entifier entifier, World world)
		{
			VersionAndId = entifier.VersionAndId;
			World = world;
		}

		public Entity(long versionAndId, World world)
		{
			VersionAndId = versionAndId;
			World = world;
		}

		public Entity(int id, uint version, World world)
		{
			VersionAndId = (uint)id | ((long)version << 32);
			World = world;
		}

		/// <summary>
		/// Checks whether the entity is alive.
		/// </summary>
		/// <remarks>
		/// Throws if provided entity ID is negative.
		/// </remarks>
		public bool IsAlive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => World.Entities.IsAlive(Entifier);
		}

		public Entifier Entifier
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entifier(VersionAndId);
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
		public bool Equals(Entifier other)
		{
			return VersionAndId == other.VersionAndId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Entity other)
		{
			return VersionAndId == other.VersionAndId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Entifier other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return VersionAndId.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return $"(id:{Id} v:{Version})";
		}
	}
}

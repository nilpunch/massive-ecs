using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Entity : IEquatable<Entity>
	{
		/// <summary>
		/// 0 counted as invalid.<br/>
		/// [ Version: 32 bits | ID: 32 bits ]
		/// </summary>
		public readonly long VersionAndId;

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

		private Entity(long versionAndId)
		{
			VersionAndId = versionAndId;
		}

		public Entity(int id, uint version)
		{
			VersionAndId = (uint)id | ((long)version << 32);
		}

		public static Entity Dead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entity(0);
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
		public bool Equals(Entity other)
		{
			return VersionAndId == other.VersionAndId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Entity other && Equals(other);
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

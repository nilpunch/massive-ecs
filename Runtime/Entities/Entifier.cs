using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Entity identifier.
	/// </summary>
	public readonly struct Entifier : IEquatable<Entifier>
	{
		/// <summary>
		/// 0 counted as invalid and dead.<br/>
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

		public Entifier(long versionAndId)
		{
			VersionAndId = versionAndId;
		}

		public Entifier(int id, uint version)
		{
			VersionAndId = (uint)id | ((long)version << 32);
		}

		public static Entifier Dead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entifier(0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity In(World world)
		{
			return new Entity(this, world);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Entifier a, Entifier b)
		{
			return a.Equals(b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Entifier a, Entifier b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Entifier other)
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

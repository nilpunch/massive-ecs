using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Entity : IEquatable<Entity>
	{
		private const int DefaultId = Constants.InvalidId;
		public const int IdOffset = -DefaultId;

		public readonly long IdAndVersion;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IdWithOffset - IdOffset;
		}

		public int IdWithOffset
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)(IdAndVersion & 0x00000000FFFFFFFF);
		}

		public uint Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)(IdAndVersion >> 32);
		}

		private Entity(long idAndVersion)
		{
			IdAndVersion = idAndVersion;
		}

		public static Entity Dead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entity(0);
		}

#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Create(int id, uint version)
		{
			long idAndVersion = (id + IdOffset) | ((long)version << 32);
			return new Entity(idAndVersion);
		}
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand

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
			return IdAndVersion == other.IdAndVersion;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Entity other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return IdAndVersion.GetHashCode();
		}
	}
}

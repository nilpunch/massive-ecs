using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Entity : IEquatable<Entity>
	{
		private const int DefaultId = Constants.InvalidId;
		public const int IdOffset = -DefaultId;

		public readonly long IdAndReuse;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IdWithOffset - IdOffset;
		}

		public int IdWithOffset
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)(IdAndReuse & 0x00000000FFFFFFFF);
		}

		public uint ReuseCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)(IdAndReuse >> 32);
		}

		private Entity(long idAndReuse)
		{
			IdAndReuse = idAndReuse;
		}

		public static Entity Dead => new Entity(0);

#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Create(int id, uint reuseCount)
		{
			long packedIdAndReuse = (id + IdOffset) | ((long)reuseCount << 32);
			return new Entity(packedIdAndReuse);
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
			return IdAndReuse == other.IdAndReuse;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Entity other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return MathHelpers.CombineHashes(Id, unchecked((int)ReuseCount));
		}
	}
}

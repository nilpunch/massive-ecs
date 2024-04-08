using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Entity : IEquatable<Entity>
	{
		public readonly int Id;
		public readonly uint ReuseCount;

		public Entity(int id, uint reuseCount)
		{
			Id = id;
			ReuseCount = reuseCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity Reuse(Entity entity)
		{
			unchecked
			{
				return new Entity(entity.Id, entity.ReuseCount + 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Entity a, Entity b)
		{
			return a.Id == b.Id && a.ReuseCount == b.ReuseCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Entity a, Entity b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Entity other)
		{
			return Id == other.Id && ReuseCount == other.ReuseCount;
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
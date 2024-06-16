using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Entity : IEquatable<Entity>
	{
		private const int DefaultId = Constants.InvalidId;
		public const int IdOffset = -DefaultId;
		
		public readonly int IdWithOffset;
		public readonly uint ReuseCount;

		public int Id => IdWithOffset - IdOffset;

		public Entity(int id, uint reuseCount)
		{
			IdWithOffset = id + IdOffset;
			ReuseCount = reuseCount;
		}

		public static Entity Dead => new Entity();

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
			return IdWithOffset == other.IdWithOffset && ReuseCount == other.ReuseCount;
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

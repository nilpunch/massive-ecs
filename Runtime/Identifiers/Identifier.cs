using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Identifier : IEquatable<Identifier>
	{
		public readonly int Id;
		public readonly uint Generation;

		public Identifier(int id, uint generation)
		{
			Id = id;
			Generation = generation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Identifier IncreaseGeneration(Identifier identifier)
		{
			unchecked
			{
				return new Identifier(identifier.Id, identifier.Generation + 1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Identifier a, Identifier b)
		{
			return a.Id == b.Id && a.Generation == b.Generation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Identifier a, Identifier b)
		{
			return !(a == b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Identifier other)
		{
			return Id == other.Id && Generation == other.Generation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj)
		{
			return obj is Identifier other && Equals(other);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return HashCode.Combine(Id, Generation);
		}
	}
}
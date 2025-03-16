﻿using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public readonly struct Entity : IEquatable<Entity>
	{
		public readonly long IdAndVersion;

		public int Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => IdWithOffset - Constants.IdOffset;
		}

		public int IdWithOffset
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (int)IdAndVersion;
		}

		public uint Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (uint)(IdAndVersion >> 32);
		}

		public Entity(int id, uint version)
		{
			IdAndVersion = (uint)(id + Constants.IdOffset) | ((long)version << 32);
		}

		public static Entity Dead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Entity(0, 0);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return $"Entity(id:{Id} v:{Version})";
		}
	}
}

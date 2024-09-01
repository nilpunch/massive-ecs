using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class InPlaceSet : ISet
	{
		private bool[] _places;

		public int UsedPlacesCount { get; set; }

		public InPlaceSet(int setCapacity = Constants.DefaultCapacity)
		{
			_places = new bool[setCapacity];
		}

		public bool[] Places
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _places;
		}

		public int PlacesCapacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Places.Length;
		}

		public event Action<int> AfterAssigned;

		public event Action<int> BeforeUnassigned;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void Assign(int id)
		{
			// If ID is negative or element is alive, nothing to be done
			if (id < 0 || id < UsedPlacesCount && Places[id])
			{
				return;
			}

			if (id >= PlacesCapacity)
			{
				EnsurePlacesCapacity(id + 1);
			}

			if (id >= UsedPlacesCount)
			{
				UsedPlacesCount = id + 1;
			}

			Places[id] = true;

			AfterAssigned?.Invoke(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Unassign(int id)
		{
			// If ID is negative or element is not alive, nothing to be done
			if (id < 0 || id >= UsedPlacesCount || !Places[id])
			{
				return;
			}

			BeforeUnassigned?.Invoke(id);

			Places[id] = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			var places = Places;
			for (int id = UsedPlacesCount - 1; id >= 0; id--)
			{
				if (places[id])
				{
					BeforeUnassigned?.Invoke(id);
					places[id] = false;
				}
			}
			UsedPlacesCount = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAssigned(int id)
		{
			return id >= 0 && id < UsedPlacesCount && Places[id];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void ResizePlaces(int capacity)
		{
			Array.Resize(ref _places, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsurePlacesCapacity(int capacity)
		{
			if (capacity > PlacesCapacity)
			{
				ResizePlaces(MathHelpers.NextPowerOf2(capacity));
			}
		}
	}
}

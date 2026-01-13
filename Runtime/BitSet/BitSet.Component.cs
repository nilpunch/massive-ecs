using System.Runtime.CompilerServices;

namespace Massive
{
	public partial class BitSet
	{
		/// <summary>
		/// Associated type index for global lookup. This value is session-dependent.
		/// </summary>
		public int TypeId { get; private set; } = -1;

		/// <summary>
		/// Associated component index for <see cref="Components"/> masks.<br/>
		/// Assigned after the first modification operation on the set.
		/// </summary>
		public int ComponentId { get; private set; } = -1;

		public int ComponentIndex { get; private set; } = -1;
		public ulong ComponentMask { get; private set; }
		public ulong ComponentMaskNegative { get; private set; }

		public bool IsComponentBound
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ComponentId >= 0;
		}

		/// <summary>
		/// Shortcut to access sets.
		/// </summary>
		private Sets Sets { get; set; }

		/// <summary>
		/// Shortcut to access components.
		/// </summary>
		private Components Components { get; set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetupComponent(Sets sets, Components components, int typeId)
		{
			Sets = sets;
			Components = components;
			TypeId = typeId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void BindComponentId(int componentId)
		{
			NegativeArgumentException.ThrowIfNegative(componentId);

			if (componentId == ComponentId)
			{
				return;
			}

			ComponentId = componentId;
			ComponentIndex = componentId >> 6;
			ComponentMask = 1UL << (componentId & 63);
			ComponentMaskNegative = ~ComponentMask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnbindComponentId()
		{
			ComponentId = -1;
			ComponentIndex = -1;
			ComponentMask = 0;
			ComponentMaskNegative = 0;
		}
	}
}

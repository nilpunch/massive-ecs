using System.Runtime.CompilerServices;

namespace Massive
{
	public partial class BitSet
	{
		/// <summary>
		/// Associated type index for global lookup. Session-dependent.
		/// </summary>
		public int TypeId { get; private set; } = -1;

		/// <summary>
		/// Associated component index for world lookup. World-dependent.
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
		/// Shortcut to access components.
		/// </summary>
		private Components Components { get; set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void BindComponent(Components components, int typeId, int componentId)
		{
			Components = components;
			TypeId = typeId;
			RebindComponent(componentId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RebindComponent(int componentId)
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
		public void UnbindComponent()
		{
			ComponentId = -1;
			ComponentIndex = -1;
			ComponentMask = 0;
			ComponentMaskNegative = 0;
		}
	}
}

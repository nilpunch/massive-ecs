using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class Registry
	{
		public BitsetSet BitsetSet { get; }
		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }
		public GroupRegistry GroupRegistry { get; }
		public Entities Entities { get; }

		public Registry(int setCapacity = Constants.DefaultCapacity, bool storeEmptyTypesAsDataSets = true, int pageSize = Constants.DefaultPageSize)
			: this(new NormalSetFactory(setCapacity, storeEmptyTypesAsDataSets, pageSize), new NormalGroupFactory(setCapacity),
				new Entities(setCapacity), new BitsetSet(capacity: setCapacity))
		{
		}

		protected Registry(ISetFactory setFactory, IGroupFactory groupFactory, Entities entities, BitsetSet bitsetSet)
		{
			SetRegistry = new SetRegistry(setFactory);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory);
			FilterRegistry = new FilterRegistry(SetRegistry);
			Entities = entities;
			BitsetSet = bitsetSet;

			// Entities.BeforeDestroyed += UnassignFromAllSets;
			// return;
			
			// Connect entity sets
			Entities.SparseResized += BitsetSet.Resize;
			SetRegistry.SetCreated += OnSetCreated;
			Entities.BeforeDestroyed += UnassignFromAllSetsUsingEntitySets;
		}

		private void OnSetCreated(SparseSet set, int setId)
		{
			//set.AttachBitset(BitsetSet, setId);
			set.AfterAssigned += entityId => BitsetSet.AssignBit(entityId, setId);
			set.BeforeUnassigned += entityId => BitsetSet.UnassignBit(entityId, setId);
		}

		private void UnassignFromAllSetsUsingEntitySets(int entityId)
		{
			var sets = BitsetSet.GetAllBits(entityId);
			for (var i = 0; i < sets.Length; i++)
			{
				SetRegistry.FindSetById(sets[i]).Unassign(entityId);
			}
		}

		private void UnassignFromAllSets(int entityId)
		{
			var sets = SetRegistry.All;
			for (var i = 0; i < sets.Length; i++)
			{
				sets[i].Unassign(entityId);
			}
		}
	}
}

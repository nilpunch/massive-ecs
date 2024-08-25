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

		public Registry()
			: this(new RegistryConfig())
		{
		}

		public Registry(RegistryConfig registryConfig)
			: this(new NormalSetFactory(registryConfig.SetCapacity, registryConfig.StoreEmptyTypesAsDataSets, registryConfig.DataPageSize),
				new NormalGroupFactory(registryConfig.SetCapacity), new Entities(registryConfig.SetCapacity),
				registryConfig.UseBitsets ? new BitsetSet(registryConfig.SetCapacity, registryConfig.BitsetMaxSetsPerEntity, registryConfig.BitsetMaxDifferentSets) : null,
				registryConfig.MaxTypesAmount)
		{
		}

		protected Registry(ISetFactory setFactory, IGroupFactory groupFactory, Entities entities, BitsetSet bitsetSet, int maxTypesAmount = Constants.DefaultMaxTypesAmount)
		{
			SetRegistry = new SetRegistry(setFactory, maxTypesAmount);
			GroupRegistry = new GroupRegistry(SetRegistry, groupFactory);
			FilterRegistry = new FilterRegistry(SetRegistry);
			Entities = entities;
			BitsetSet = bitsetSet;

			if (BitsetSet is not null)
			{
				Entities.SparseResized += BitsetSet.Resize;
				SetRegistry.SetCreated += ConnectBitset;
				Entities.BeforeDestroyed += UnassignFromAllSetsUsingBitset;
			}
			else
			{
				Entities.BeforeDestroyed += UnassignFromAllSets;
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

		private void UnassignFromAllSetsUsingBitset(int entityId)
		{
			var sets = BitsetSet.GetAllBits(entityId);
			for (var i = 0; i < sets.Length; i++)
			{
				SetRegistry.Find(sets[i]).UnassignUnsafe(entityId);
			}
		}

		private void ConnectBitset(SparseSet set, int setId)
		{
			set.AfterAssigned += entityId => { BitsetSet.AssignBit(entityId, setId); };
			set.BeforeUnassigned += entityId => { BitsetSet.UnassignBit(entityId, setId); };
		}
	}
}

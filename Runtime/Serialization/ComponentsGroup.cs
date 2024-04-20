using System;
using System.Collections.Generic;

namespace Massive.Serialization
{
	public class ComponentsGroup
	{
		public virtual IReadOnlyList<IReadOnlySet> GetMany(IRegistry registry)
		{
			return Array.Empty<IReadOnlySet>();
		}
	}

	public class ComponentsGroup<T> : ComponentsGroup where T : struct
	{
		public override IReadOnlyList<IReadOnlySet> GetMany(IRegistry registry)
		{
			return registry.Many<T>();
		}
	}

	public class ComponentsGroup<T1, T2> : ComponentsGroup where T1 : struct where T2 : struct
	{
		public override IReadOnlyList<IReadOnlySet> GetMany(IRegistry registry)
		{
			return registry.Many<T1, T2>();
		}
	}

	public class ComponentsGroup<T1, T2, T3> : ComponentsGroup where T1 : struct where T2 : struct where T3 : struct
	{
		public override IReadOnlyList<IReadOnlySet> GetMany(IRegistry registry)
		{
			return registry.Many<T1, T2, T3>();
		}
	}
}
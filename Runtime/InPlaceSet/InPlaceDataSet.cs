using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.InPlaceSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class InPlaceDataSet<T> : InPlaceSet, IDataSet<T>
	{
		public PagedArray<T> Data { get; }

		public InPlaceDataSet(int setCapacity = Constants.DefaultCapacity, int pageSize = Constants.DefaultPageSize)
			: base(setCapacity)
		{
			Data = new PagedArray<T>(pageSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Assign(int id)
		{
			Data.EnsurePageForIndex(id);
			base.Assign(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyData(int sourceId, int targetId)
		{
			Get(targetId) = Get(sourceId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Data[id];
		}
	}
}

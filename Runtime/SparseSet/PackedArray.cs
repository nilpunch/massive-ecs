using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class PackedArray<T>
	{
		private readonly int _pageSize;
		private T[][] _pagedData;

		public PackedArray(int pageSize = Constants.PageSize)
		{
			if (!MathHelpers.IsPowerOfTwo(pageSize))
			{
				throw new Exception("Page capacity must be power of two!");
			}

			_pageSize = pageSize;
			_pagedData = new T[Constants.PagesAmount][];
		}

		public T[][] PagedData => _pagedData;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int index)
		{
			int page = index / _pageSize;
			return ref PagedData[page][MathHelpers.FastMod(index, _pageSize)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetSafe(int index)
		{
			int page = index / _pageSize;

			EnsurePage(page);

			return ref PagedData[page][MathHelpers.FastMod(index, _pageSize)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePageForIndex(int index)
		{
			int page = index / _pageSize;

			EnsurePage(page);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsurePage(int page)
		{
			if (page >= _pagedData.Length)
			{
				Array.Resize(ref _pagedData, MathHelpers.GetNextPowerOf2(page));
			}

			_pagedData[page] ??= new T[_pageSize];
		}
	}
}

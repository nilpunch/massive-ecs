using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public partial class QueryCache
	{
		[ThreadStatic]
		private static QueryCache[] _cachePool;

		[ThreadStatic]
		private static int _poolCount;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static QueryCache Rent()
		{
			if (_poolCount > 0)
			{
				return _cachePool[--_poolCount];
			}

			return new QueryCache();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReturnAndPop(QueryCache queryCache)
		{
			queryCache.Pop();

			var pool = _cachePool ??= new QueryCache[8];
			if (_poolCount >= pool.Length)
			{
				Array.Resize(ref pool, MathUtils.RoundUpToPowerOfTwo(_poolCount + 1));
				_cachePool = pool;
			}
			pool[_poolCount++] = queryCache;
		}
	}
}

using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public partial class BitSetBase
	{
		protected QueryCache[] RemoveOnAdd { get; private set; } = Array.Empty<QueryCache>();
		protected int RemoveOnAddCount { get; private set; }

		protected QueryCache[] RemoveOnRemove { get; private set; } = Array.Empty<QueryCache>();
		protected int RemoveOnRemoveCount { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnAdd(QueryCache queryCache)
		{
			if (RemoveOnAddCount >= RemoveOnAdd.Length)
			{
				RemoveOnAdd = RemoveOnAdd.ResizeToNextPowOf2(RemoveOnAddCount + 1);
			}

			RemoveOnAdd[RemoveOnAddCount++] = queryCache;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnAdd()
		{
			RemoveOnAddCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PushRemoveOnRemove(QueryCache queryCache)
		{
			if (RemoveOnRemoveCount >= RemoveOnRemove.Length)
			{
				RemoveOnRemove = RemoveOnRemove.ResizeToNextPowOf2(RemoveOnRemoveCount + 1);
			}

			RemoveOnRemove[RemoveOnRemoveCount++] = queryCache;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopRemoveOnRemove()
		{
			RemoveOnRemoveCount--;
		}
	}
}

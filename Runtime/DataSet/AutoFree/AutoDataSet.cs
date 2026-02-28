#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using Unity.IL2CPP.CompilerServices;

namespace Massive.AutoFree
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class AutoDataSet<T> : DataSet<T> where T : unmanaged
	{
		private readonly Allocator _allocator;
		private readonly AllocatorTypeSchema _schema;

		public AutoDataSet(Allocator allocator, T defaultValue = default) : base(defaultValue)
		{
			_allocator = allocator;
			_schema = AllocatorTypeSchema<T>.Schema;
		}

		protected override unsafe void ClearData(int id)
		{
			fixed (T* page = PagedData[id >> Constants.PageSizePower])
			{
				var data = page + (id & Constants.PageSizeMinusOne);
				_schema.DeepFreePointedData(_allocator, (byte*)data);
				*data = DefaultValue;
			}
		}

		public override unsafe void CopyData(int sourceId, int destinationId)
		{
			InvalidGetOperationException.ThrowIfNotAdded(this, destinationId);
			InvalidGetOperationException.ThrowIfNotAdded(this, sourceId);

			fixed (T* sourcePage = PagedData[sourceId >> Constants.PageSizePower])
			{
				fixed (T* destinationPage = PagedData[destinationId >> Constants.PageSizePower])
				{
					var sourceData = sourcePage + (sourceId & Constants.PageSizeMinusOne);
					var destinationData = destinationPage + (destinationId & Constants.PageSizeMinusOne);
					*destinationData = *sourceData;
					_schema.DeepCopyPointedData(_allocator, (byte*)sourceData, (byte*)destinationData);
				}
			}
		}
	}
}

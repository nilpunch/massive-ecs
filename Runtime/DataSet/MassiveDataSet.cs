using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.DataSet{T}"/>.
	/// Resets data when elemets are moved.
	/// Used in registry for unmanaged components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveDataSet<T> : DataSet<T>, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;

		private readonly PagedArray<T>[] _dataByFrames;
		private readonly int[][] _packedByFrames;
		private readonly int[][] _sparseByFrames;
		private readonly State[] _stateByFrames;

		public MassiveDataSet(int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous) : base(pageSize, packing)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new PagedArray<T>[framesCapacity];
			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_stateByFrames = new State[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_dataByFrames[i] = new PagedArray<T>(pageSize);
				_packedByFrames[i] = Array.Empty<int>();
				_sparseByFrames[i] = Array.Empty<int>();
			}
		}

		public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			CopyData(Data, _dataByFrames[currentFrame], Count);
			Array.Copy(Packed, _packedByFrames[currentFrame], Count);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], SparseCapacity);
			_stateByFrames[currentFrame] = CurrentState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			var rollbackSparseCapacity = _sparseByFrames[rollbackFrame].Length;
			var rollbackState = _stateByFrames[rollbackFrame];

			CopyData(_dataByFrames[rollbackFrame], Data, rollbackState.Count);
			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackState.Count);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseCapacity);
			if (rollbackSparseCapacity < SparseCapacity)
			{
				Array.Fill(Sparse, Constants.InvalidId, rollbackSparseCapacity, SparseCapacity - rollbackSparseCapacity);
			}
			CurrentState = rollbackState;
		}

		protected virtual void CopyData(PagedArray<T> source, PagedArray<T> destination, int count)
		{
			foreach (var page in new PageSequence(source.PageSize, count))
			{
				if (!source.HasPage(page.Index))
				{
					continue;
				}

				destination.EnsurePage(page.Index);

				var sourcePage = source.Pages[page.Index];
				var destinationPage = destination.Pages[page.Index];

				Array.Copy(sourcePage, destinationPage, page.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_sparseByFrames[frame].Length < SparseCapacity)
			{
				var previousCapacity = _sparseByFrames[frame].Length;
				Array.Resize(ref _sparseByFrames[frame], SparseCapacity);
				Array.Fill(_sparseByFrames[frame], Constants.InvalidId, previousCapacity, SparseCapacity - previousCapacity);
			}

			if (_packedByFrames[frame].Length < PackedCapacity)
			{
				Array.Resize(ref _packedByFrames[frame], PackedCapacity);
			}
		}
	}
}

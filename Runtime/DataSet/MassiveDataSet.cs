using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.DataSet{T}"/>.
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
			int pageSize = Constants.DefaultPageSize, PackingMode packingMode = PackingMode.Continuous) : base(pageSize, packingMode)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);

			_dataByFrames = new PagedArray<T>[framesCapacity];
			_packedByFrames = new int[framesCapacity][];
			_sparseByFrames = new int[framesCapacity][];
			_stateByFrames = new State[framesCapacity];

			for (int i = 0; i < framesCapacity; i++)
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

			int currentFrame = _cyclicFrameCounter.CurrentFrame;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame
			CopyData(Data, _dataByFrames[currentFrame], Count);
			Array.Copy(Packed, _packedByFrames[currentFrame], Count);
			Array.Copy(Sparse, _sparseByFrames[currentFrame], Sparse.Length);
			_stateByFrames[currentFrame] = CurrentState;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state
			int rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			int rollbackSparseLength = _sparseByFrames[rollbackFrame].Length;
			State rollbackState = _stateByFrames[rollbackFrame];

			CopyData(_dataByFrames[rollbackFrame], Data, rollbackState.Count);
			Array.Copy(_packedByFrames[rollbackFrame], Packed, rollbackState.Count);
			Array.Copy(_sparseByFrames[rollbackFrame], Sparse, rollbackSparseLength);
			if (rollbackSparseLength < Sparse.Length)
			{
				Array.Fill(Sparse, Constants.InvalidId, rollbackSparseLength, Sparse.Length - rollbackSparseLength);
			}
			CurrentState = rollbackState;
		}

		protected virtual void CopyData(PagedArray<T> source, PagedArray<T> destination, int count)
		{
			foreach (var (pageIndex, pageLength, _) in new PageSequence(source.PageSize, count))
			{
				if (!source.HasPage(pageIndex))
				{
					continue;
				}

				destination.EnsurePage(pageIndex);

				var sourcePage = source.Pages[pageIndex];
				var destinationPage = destination.Pages[pageIndex];

				Array.Copy(sourcePage, destinationPage, pageLength);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_sparseByFrames[frame].Length < Sparse.Length)
			{
				var previousLength = _sparseByFrames[frame].Length;
				Array.Resize(ref _sparseByFrames[frame], Sparse.Length);
				Array.Fill(_sparseByFrames[frame], Constants.InvalidId, previousLength, Sparse.Length - previousLength);
			}

			if (_packedByFrames[frame].Length < Packed.Length)
			{
				Array.Resize(ref _packedByFrames[frame], Packed.Length);
			}
		}
	}
}

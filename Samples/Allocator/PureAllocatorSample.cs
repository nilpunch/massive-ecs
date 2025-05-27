namespace Massive.Samples.Basic
{
	class PureAllocatorSample
	{
		Allocator<int> Allocator;
		ChunkHandle<int> Values;

		void Allocate()
		{
			Allocator = new Allocator<int>();

			// Allocate an empty chunk (same as new int[0]).
			var chunk = Allocator.AllocChunk(0);

			// Store the chunk using a handle.
			// Handles are unmanaged and safe to store in simulation.
			Values = chunk;
		}

		void Use()
		{
			// To access the chunk, combine the handle with the allocator.
			var values = Values.In(Allocator);

			// Resize the chunk whenever needed. The handle stays valid.
			values.Resize(2);

			// Use it like a normal array: index, iterate, modify, copy.
			values[0] = 5;

			foreach (ref var value in values)
			{
				value += 1;
			}
		}

		void Free()
		{
			// Free the chunk when you're done with it.
			Allocator.Free(Values);
		}
	}
}

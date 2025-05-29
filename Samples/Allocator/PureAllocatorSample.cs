namespace Massive.Samples.Basic
{
	class PureAllocatorSample
	{
		Allocator<int> Allocator;
		ArrayHandle<int> Values;

		void Allocate()
		{
			Allocator = new Allocator<int>();

			// Allocate an empty array (same as new int[0]).
			var array = Allocator.AllocArray(0);

			// Store the array using a handle.
			// Handles are unmanaged and safe to store in simulation.
			Values = array;
		}

		void Use()
		{
			// To access the array, combine the handle with the allocator.
			var values = Values.In(Allocator);

			// Resize the array whenever needed. The handle stays valid.
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
			// Free the array when you're done with it.
			Allocator.Free(Values);
		}
	}
}

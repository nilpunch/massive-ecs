using System.Runtime.CompilerServices;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct PhysicsRigidbody
	{
		public Transformation WorldCenterOfMass;

		public Vector3 Velocity;
		public Vector3 Forces;

		public Vector3 AngularVelocity;
		public Vector3 Torques;

		public Matrix4x4 LocalInertiaTensor;
		public Matrix4x4 InverseWorldInertiaTensor;
		public float InverseMass;
		public float Mass;

		public bool IsStatic;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Integrate(float deltaTime)
		{
			if (IsStatic)
			{
				Velocity = Vector3.zero;
				Forces = Vector3.zero;
				return;
			}

			// Linear motion integration
			Velocity += deltaTime * InverseMass * Forces;
			WorldCenterOfMass.Position += deltaTime * Velocity;
			Forces = Vector3.zero;

			// Angular motion integration
			AngularVelocity += deltaTime * InverseWorldInertiaTensor.MultiplyPoint3x4(Torques);
			WorldCenterOfMass.Rotation = Quaternion.Euler(deltaTime * Mathf.Rad2Deg * AngularVelocity) * WorldCenterOfMass.Rotation;
			Torques = Vector3.zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyImpulseAtPoint(Vector3 impulse, Vector3 position)
		{
			if (!IsStatic)
			{
				// Apply linear impulse to the velocity
				Velocity += impulse * InverseMass;

				// Calculate the lever arm from the center of mass to the point of application
				Vector3 leverArm = position - WorldCenterOfMass.Position;

				// Calculate the angular impulse using the cross product between the lever arm and the impulse
				Vector3 angularImpulse = Vector3.Cross(leverArm, impulse);

				// Apply angular impulse to the angular velocity
				AngularVelocity += InverseWorldInertiaTensor.MultiplyPoint3x4(angularImpulse);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ApplyForce(Vector3 force)
		{
			Forces += force * Mass;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateWorldInertiaTensor()
		{
			var rotation = Matrix4x4.Rotate(WorldCenterOfMass.Rotation);
			InverseWorldInertiaTensor = Matrix4x4.Inverse(rotation * LocalInertiaTensor * rotation.transpose);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetVelocityAtPoint(Vector3 point)
		{
			return Velocity + Vector3.Cross(AngularVelocity, point - WorldCenterOfMass.Position);
		}

		public static void IntegrateAll(IReadOnlyDataSet<PhysicsRigidbody> bodies, float deltaTime)
		{
			var aliveRigidbodies = bodies.AliveData;
			for (var i = 0; i < aliveRigidbodies.Length; i++)
			{
				aliveRigidbodies[i].Integrate(deltaTime);
			}
		}

		public static void UpdateAllWorldInertia(IReadOnlyDataSet<PhysicsRigidbody> bodies)
		{
			var bodiesAlive = bodies.AliveData;
			for (int i = 0; i < bodiesAlive.Length; i++)
			{
				bodiesAlive[i].UpdateWorldInertiaTensor();
			}
		}

		public static void RecalculateAllInertia(IReadOnlyDataSet<PhysicsRigidbody> bodies, IReadOnlyDataSet<PhysicsBoxCollider> boxes, IReadOnlyDataSet<PhysicsSphereCollider> spheres)
		{
			var bodiesAlive = bodies.AliveData;
			var boxesAlive = boxes.AliveData;
			var spheresAlive = spheres.AliveData;

			for (int i = 0; i < bodiesAlive.Length; i++)
			{
				bodiesAlive[i].Mass = 0f;
				bodiesAlive[i].LocalInertiaTensor = Matrix4x4.zero;
			}

			foreach (var box in boxesAlive)
			{
				ref var body = ref bodies.Get(box.RigidbodyId);
				body.LocalInertiaTensor = CombineInertiaTensor(body.LocalInertiaTensor, box.LocalInertiaTensor());
				body.Mass = box.Mass();
			}

			foreach (var sphere in spheresAlive)
			{
				ref var body = ref bodies.Get(sphere.RigidbodyId);
				body.LocalInertiaTensor = CombineInertiaTensor(body.LocalInertiaTensor, sphere.LocalInertiaTensor());
				body.Mass = sphere.Mass();
			}

			for (int i = 0; i < bodiesAlive.Length; i++)
			{
				bodiesAlive[i].InverseMass = 1f / bodiesAlive[i].Mass;
			}
		}

		public static Matrix4x4 TransformInertiaTensor(Matrix4x4 moi, Vector3 offsetFromCenterOfMass, Quaternion localRotation, float mass)
		{
			var rotation = Matrix4x4.Rotate(localRotation);

			moi = rotation * moi * rotation.transpose;

			// Apply the parallel axis theorem to adjust MOI for the collider's offset
			float distanceSquared = offsetFromCenterOfMass.sqrMagnitude;

			// Calculate the additional inertia using the parallel axis theorem: d^2 * m
			float additionalInertia = distanceSquared * mass;

			moi[0, 0] += additionalInertia;
			moi[1, 1] += additionalInertia;
			moi[2, 2] += additionalInertia;

			return moi;
		}

		public static Matrix4x4 CombineInertiaTensor(Matrix4x4 tensorA, Matrix4x4 tensorB)
		{
			Matrix4x4 sumTensor = new Matrix4x4();

			// Iterate only over the first 3 rows and columns for the inertia tensor
			for (int i = 0; i < 3; i++) // Iterate through rows
			{
				for (int j = 0; j < 3; j++) // Iterate through columns
				{
					// Sum the corresponding elements of tensorA and tensorB
					sumTensor[i, j] = tensorA[i, j] + tensorB[i, j];
				}
			}

			// Set the last row and column for completeness, maintaining structure
			sumTensor[3, 3] = 1; // For a proper homogeneous transformation matrix
			for (int k = 0; k < 3; k++)
			{
				sumTensor[k, 3] = sumTensor[3, k] = 0; // Clearing the rest as they are not used
			}

			return sumTensor;
		}
	}
}
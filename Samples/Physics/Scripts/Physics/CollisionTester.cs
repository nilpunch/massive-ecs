using System.Runtime.CompilerServices;
using UnityEngine;

namespace MassiveData.Samples.Physics
{
	public static class CollisionTester
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SphereVsSphere(ref SphereCollider a, ref SphereCollider b, Vector3 offsetToB, ref ColliderContact colliderContact)
		{
			var centerDistance = offsetToB.magnitude;
			
			colliderContact.Normal = -offsetToB / centerDistance;

			bool normalIsValid = centerDistance > 0f;

			// Arbitrarily choose the (0,1,0) if the two spheres are in the same position
			// Any unit length vector is equally valid
			if (!normalIsValid)
				colliderContact.Normal = Vector3.up;

			// Calculate depth
			colliderContact.Depth = a.Radius + b.Radius - centerDistance;

			// Calculate the contact position relative to object A
			float negativeOffsetFromA = colliderContact.Depth * 0.5f - a.Radius;

			colliderContact.OffsetFromColliderA = colliderContact.Normal * negativeOffsetFromA;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SphereVsBox(ref SphereCollider a, ref BoxCollider b, Vector3 offsetToB, Quaternion orientationOfB, ref ColliderContact colliderContact)
		{
			// Note that we're working with localOffsetB, which is the offset from A to B, even though conceptually we want to be operating on the offset from B to A
			// Those offsets differ only by their sign, so are equivalent due to the symmetry of the box. The negation is left implicit
			var localOffsetB = Quaternion.Inverse(orientationOfB) * offsetToB;
			Vector3 clampedLocalOffsetB;
			
			clampedLocalOffsetB.x = Mathf.Min(Mathf.Max(localOffsetB.x, -b.HalfSize.x), b.HalfSize.x);
			clampedLocalOffsetB.y = Mathf.Min(Mathf.Max(localOffsetB.y, -b.HalfSize.y), b.HalfSize.y);
			clampedLocalOffsetB.z = Mathf.Min(Mathf.Max(localOffsetB.z, -b.HalfSize.z), b.HalfSize.z);

			// Implicit negation to make the normal point from B to A, following convention
			var outsideNormal = clampedLocalOffsetB - localOffsetB;
			var distance = outsideNormal.magnitude;
			var inverseDistance = 1f / distance;
			outsideNormal *= inverseDistance;
			var outsideDepth = a.Radius - distance;

			// If the sphere center is inside the box, then the shortest local axis to exit must be chosen
			var depthX = b.HalfSize.x - Mathf.Abs(localOffsetB.x);
			var depthY = b.HalfSize.y - Mathf.Abs(localOffsetB.y);
			var depthZ = b.HalfSize.z - Mathf.Abs(localOffsetB.z);
			var insideDepth = Mathf.Min(depthX, Mathf.Min(depthY, depthZ));

			// Only one axis may have a nonzero component
			bool useX = insideDepth == depthX;
			bool useY = insideDepth == depthY && !useX;
			bool useZ = !(useX || useY);

			var insideNormal = Vector3.zero;

			if (useX)
				insideNormal.x = (localOffsetB.x < 0f) ? 1f : -1f;
			if (useY)
				insideNormal.y = (localOffsetB.y < 0f) ? 1f : -1f;
			if (useZ)
				insideNormal.z = (localOffsetB.z < 0f) ? 1f : -1f;

			insideDepth += a.Radius;
			var useInside = distance == 0f;
			var localNormal = useInside ? insideNormal : outsideNormal;
			colliderContact.Normal = orientationOfB * localNormal;

			colliderContact.Depth = useInside ? insideDepth : outsideDepth;

			//The contact position relative to object A (the sphere) is computed as the average of the extreme point along the normal toward the opposing shape on each shape, averaged.
			//For capsule-sphere, this can be computed from the normal and depth.
			var negativeOffsetFromSphere = colliderContact.Depth * 0.5f - a.Radius;
			colliderContact.OffsetFromColliderA = colliderContact.Normal * negativeOffsetFromSphere;
		}
	}
}
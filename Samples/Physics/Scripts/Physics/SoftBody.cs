using System.Collections.Generic;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public struct SoftBody
	{
		public Vector3 Centroid;
		public Matrix4x4 Rotation;

		private static readonly List<List<PointMass>> s_softBodiesPoints = new List<List<PointMass>>();

		public static void UpdateAll(MassiveData<SoftBody> softBodies, MassiveData<PointMass> points)
		{
			// Clear cache
			foreach (var softBodyPoints in s_softBodiesPoints)
			{
				softBodyPoints.Clear();
			}

			var pointsData = points.Data;
			var softBodiesData = softBodies.Data;
			int alivePoints = points.AliveCount;
			int aliveBodies = softBodies.AliveCount;

			// Up cache capacity
			for (int i = s_softBodiesPoints.Count; i < aliveBodies; i++)
			{
				s_softBodiesPoints.Add(new List<PointMass>());
			}

			// Collect bodies points
			for (int i = 0; i < alivePoints; i++)
			{
				var pointMass = pointsData[i];
				int denseBody = softBodies.GetDenseIndex(pointMass.SoftBodyId);
				s_softBodiesPoints[denseBody].Add(pointMass);
			}

			// Calculate center
			for (int bodyIndex = 0; bodyIndex < aliveBodies; bodyIndex++)
			{
				var bodyPoints = s_softBodiesPoints[bodyIndex];
				int pointsCount = bodyPoints.Count;

				// Calculate centorid
				Vector3 centroid = Vector3.zero;
				for (int j = 0; j < pointsCount; j++)
				{
					centroid += bodyPoints[j].Position;
				}

				softBodiesData[bodyIndex].Centroid = centroid / pointsCount;

				// Matrix4x4 covarianceMatrix = Matrix4x4.zero;
				//
				// for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
				// {
				// 	Vector3 originalPoint = bodyPoints[pointIndex].LocalReferencePosition;
				// 	Vector3 currentPoint = bodyPoints[pointIndex].Position - centroid;
				//
				// 	// Compute outer product and add to covariance matrix
				// 	covarianceMatrix.m00 += originalPoint.x * currentPoint.x;
				// 	covarianceMatrix.m01 += originalPoint.x * currentPoint.y;
				// 	covarianceMatrix.m02 += originalPoint.x * currentPoint.z;
				//
				// 	covarianceMatrix.m10 += originalPoint.y * currentPoint.x;
				// 	covarianceMatrix.m11 += originalPoint.y * currentPoint.y;
				// 	covarianceMatrix.m12 += originalPoint.y * currentPoint.z;
				//
				// 	covarianceMatrix.m20 += originalPoint.z * currentPoint.x;
				// 	covarianceMatrix.m21 += originalPoint.z * currentPoint.y;
				// 	covarianceMatrix.m22 += originalPoint.z * currentPoint.z;
				// }
				//
				// covarianceMatrix.m00 /= pointsCount;
				// covarianceMatrix.m01 /= pointsCount;
				// covarianceMatrix.m02 /= pointsCount;
				// covarianceMatrix.m10 /= pointsCount;
				// covarianceMatrix.m11 /= pointsCount;
				// covarianceMatrix.m12 /= pointsCount;
				// covarianceMatrix.m20 /= pointsCount;
				// covarianceMatrix.m21 /= pointsCount;
				// covarianceMatrix.m22 /= pointsCount;

				softBodiesData[bodyIndex].Rotation = Matrix4x4.identity; // ComputeRotationMatrix(covarianceMatrix);
			}
		}

		private static Matrix4x4 ComputeRotationMatrix(Matrix4x4 covarianceMatrix)
		{
			// Perform SVD for a 3x3 matrix
			SVD(covarianceMatrix, out Matrix4x4 U, out Vector3 S, out Matrix4x4 V);

			// Construct the rotation matrix
			Matrix4x4 rotationMatrix = U * V.transpose;

			return rotationMatrix;
		}

		// Singular Value Decomposition for a 3x3 matrix
		private static void SVD(Matrix4x4 A, out Matrix4x4 U, out Vector3 S, out Matrix4x4 V)
		{
			Matrix4x4 ATA = A.transpose * A;
			EigenDecomposition(ATA, out U, out S);

			// Compute V matrix
			V = A * U * Matrix4x4.Scale(new Vector3(1f / S.x, 1f / S.y, 1f / S.z));
		}

		// Eigenvalue decomposition for a symmetric 3x3 matrix
		private static void EigenDecomposition(Matrix4x4 A, out Matrix4x4 V, out Vector3 D)
		{
			// Initialize V as the identity matrix
			V = Matrix4x4.identity;

			// Maximum number of iterations
			int maxIterations = 100;

			for (int iteration = 0; iteration < maxIterations; iteration++)
			{
				// Find the largest off-diagonal element
				int p = 0, q = 1;
				float maxOffDiagonal = Mathf.Abs(A[0, 1]);

				if (Mathf.Abs(A[1, 2]) > maxOffDiagonal)
				{
					p = 1;
					q = 2;
					maxOffDiagonal = Mathf.Abs(A[1, 2]);
				}

				if (Mathf.Abs(A[0, 2]) > maxOffDiagonal)
				{
					p = 0;
					q = 2;
					maxOffDiagonal = Mathf.Abs(A[0, 2]);
				}

				// Check convergence
				if (maxOffDiagonal < 1e-12f)
					break;

				// Compute rotation angle
				float theta = 0.5f * Mathf.Atan2(2.0f * A[p, q], A[q, q] - A[p, p]);

				// Compute cosine and sine of rotation angle
				float c = Mathf.Cos(theta);
				float s = Mathf.Sin(theta);

				// Construct the rotation matrix
				Matrix4x4 R = Matrix4x4.identity;
				R[p, p] = c;
				R[p, q] = -s;
				R[q, p] = s;
				R[q, q] = c;

				// Update matrices
				A = R.transpose * A * R;
				V *= R;
			}

			// Extract eigenvalues
			D = new Vector3(A.m00, A.m11, A.m22);

			// Ensure eigenvalues are sorted in descending order
			if (D.y > D.x)
			{
				Swap(ref D.x, ref D.y);
				V = SwapColumns(V, 0, 1);
			}

			if (D.z > D.y)
			{
				Swap(ref D.y, ref D.z);
				V = SwapColumns(V, 1, 2);
			}

			if (D.y > D.x)
			{
				Swap(ref D.x, ref D.y);
				V = SwapColumns(V, 0, 1);
			}
		}

		// Helper method to swap two values
		private static void Swap(ref float a, ref float b)
		{
			(a, b) = (b, a);
		}

		// Helper method to swap columns of a matrix
		private static Matrix4x4 SwapColumns(Matrix4x4 matrix, int column1, int column2)
		{
			for (int i = 0; i < 4; i++)
			{
				(matrix[i, column1], matrix[i, column2]) = (matrix[i, column2], matrix[i, column1]);
			}

			return matrix;
		}
	}
}
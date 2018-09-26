using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Maths;

namespace Spline {
	public class Splines {
		public static CubicSpline HermiteSpline(HermiteForm control) {
			CubicPolynomial3 basis;
			basis.a = 2 * control.p0 - 2 * control.p1 + control.m0 + control.m1;
			basis.b = -3 * control.p0 + 3 * control.p1 - 2 * control.m0 - control.m1;
			basis.c = control.m0;
			basis.d = control.p0;

			QuadraticPolynomial3 derivative;
			derivative.a = 6 * control.p0 - 6 * control.p1 + 3 * control.m0 + 3 * control.m1;
			derivative.b = -6 * control.p0 + 6 * control.p1 - 4 * control.m0 - 2 * control.m1;
			derivative.c = control.m0;

			return new CubicSpline(basis, derivative, ArcLength(derivative));
		}

		public static void SubdivideHermiteSpline(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1, out CubicPolynomial3 m1Prime) {
			m1Prime.a = -12f * p0 - 6f * p1 - 3f * m0 + 3f * m1;
			m1Prime.b = 12f * p1 + 8f * m0 - 2f * m1;
			m1Prime.c = -48f * p0 - 5f * m0;
			m1Prime.d = 6f * p0;
		}

		public static CubicSpline CentripetalCatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
			float dt0 = Mathf.Sqrt(Vector3.Distance(p0, p1));
			float dt1 = Mathf.Sqrt(Vector3.Distance(p1, p2));
			float dt2 = Mathf.Sqrt(Vector3.Distance(p2, p3));

			// safety check for repeated points
			if(dt1 < Mathf.Epsilon) {
				dt1 = 1.0f;
			}
			if(dt0 < Mathf.Epsilon) {
				dt0 = dt1;
			}
			if(dt2 < Mathf.Epsilon) {
				dt2 = dt1;
			}

			Vector3 m1 = (p1 - p0) / dt0 - (p2 - p0) / (dt0 + dt1) + (p2 - p1) / dt1;
			Vector3 m2 = (p2 - p1) / dt1 - (p3 - p1) / (dt1 + dt2) + (p3 - p2) / dt2;

			m1 *= dt1;
			m2 *= dt1;

			CubicPolynomial3 basis = new CubicPolynomial3();
			basis.a = 2 * p1 - 2 * p2 + m1 + m2;
			basis.b = -3 * p1 + 3 * p2 - 2 * m1 - m2;
			basis.c = m1;
			basis.d = p1;

			QuadraticPolynomial3 derivative = new QuadraticPolynomial3();
			derivative.a = 6 * p1 - 6 * p2 + 3 * m1 + 3 * m2;
			derivative.b = -6 * p1 + 6 * p2 - 4 * m1 - 2 * m2;
			derivative.c = m1;

			//_arcLength = ArcLength(1f);
			//_arcOffset = arcOffset;

			return new CubicSpline();
		}

		public static float ArcLength(QuadraticPolynomial3 derivative, float tmax = 1f) {
			float prev = derivative.Solve(tmax).magnitude;

			float sum = 0f;
			for(int i = 1; i <= 10; i++) {
				float t = (i / 10f) * tmax;

				float f = derivative.Solve(t).magnitude;

				sum += tmax * (prev + f) / 10f;

				prev = f;
			}

			return  sum / 2f;
		}

		public static float GetCurveParameter(CubicSpline spline, float s) {
			float t = s / spline.arcLength; // Initial guess

			float lower = 0f, upper = 1f;
			float MAX_ITERATIONS = 10;

			for(int i = 0; i < MAX_ITERATIONS; i++) {
				float f = ArcLength(spline.derivative, t) - s;
				if(Mathf.Abs(f) < 0.001f) {
					return t;
				}

				float df = spline.derivative.Solve(t).magnitude;

				float tCandidate = t - (f / df);

				if(f > 0) {
					upper = t;
					t = (tCandidate <= lower) ? (upper + lower) / 2f : tCandidate;
				} else {
					lower = t;
					t = (tCandidate >= upper) ? (upper + lower) / 2f : tCandidate;
				}
			}

			Debug.LogWarning("Root was not found after " + MAX_ITERATIONS + " iterations");
			return t;
		}

		// Everything below here is shit

		public struct Point {
			public Vector3 position;
			public Quaternion orientation;
			public int index;
			public float len;

			public Vector3 LocalToWorld(Vector3 point) {
				return position + orientation * point;
			}

			public Vector3 WorldToLocal(Vector3 point) {
				return Quaternion.Inverse(orientation) * (point - position);
			}

			public Vector3 LocalToWorldDirection(Vector3 dir) {
				return orientation * dir;
			}
		}

		public static Point GetPoint(CubicSpline spline, float t, int index, float len) {
			Vector3 position = spline.basis.Solve(t);
			Vector3 tangent = spline.derivative.Solve(t);

			Vector3 binormal = Vector3.Cross(Vector3.up, tangent).normalized;
			Vector3 normal = Vector3.Cross(tangent, binormal);

			if(tangent.magnitude <= 0.001f || tangent == Vector3.zero || normal.magnitude <= 0.001f || normal == Vector3.zero) {
				Debug.Log ("bad thing - tangent.magnitude = " + tangent.magnitude + ", normal.magnitude = " + normal.magnitude);
			}

			Quaternion orientation = Quaternion.LookRotation(tangent, normal);

			Point p = new Point();
			p.position = position;
			p.orientation = orientation;
			p.index = index;
			p.len = len;
			return p;
		}

		public static IEnumerable<Point> Sample(CubicSpline spline, int points) {
			yield return GetPoint(spline, 0f, 0, 0f);

			for(int n = 1; n < points - 1; n++) {
				float s = (n / (points - 1f)) * spline.arcLength;

				yield return GetPoint(spline, GetCurveParameter(spline, s), n, s);
			}

			yield return GetPoint(spline, 1f, points, spline.arcLength);
		}
	}
}

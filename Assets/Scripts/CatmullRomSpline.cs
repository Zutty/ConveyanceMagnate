using UnityEngine;
using System.Collections.Generic;
using Maths;

public struct CatmullRomSpline {

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

	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;

	private CubicPolynomial3 basis;
	private QuadraticPolynomial3 derivative;
	private float _arcLength;
	private float _arcOffset;

	public void CalculateBasis(float arcOffset) {
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

		basis = new CubicPolynomial3();
		basis.a = 2 * p1 - 2 * p2 + m1 + m2;
		basis.b = -3 * p1 + 3 * p2 - 2 * m1 - m2;
		basis.c = m1;
		basis.d = p1;

		derivative = new QuadraticPolynomial3();
		derivative.a = 6 * p1 - 6 * p2 + 3 * m1 + 3 * m2;
		derivative.b = -6 * p1 + 6 * p2 - 4 * m1 - 2 * m2;
		derivative.c = m1;

		_arcLength = ArcLength(1f);
		_arcOffset = arcOffset;
	}

	public SexticPolynomialf CircleIntersection(Vector3 o, float r) {
		SexticPolynomialf intersection = new SexticPolynomialf();

		// a^2t^6
		intersection.a = (basis.a.x * basis.a.x) + (basis.a.y * basis.a.y) + (basis.a.z * basis.a.z);
		// 2abt^5
		intersection.b = 2f * ((basis.a.x * basis.b.x) + (basis.a.y * basis.b.y) + (basis.a.z * basis.b.z));
		// (2ac + b^2)t^4
		intersection.c = 2f * ((basis.a.x * basis.c.x) + (basis.a.y * basis.c.y) + (basis.a.z * basis.c.z)) + (basis.b.x * basis.b.x) + (basis.b.y * basis.b.y) + (basis.b.z * basis.b.z);

		// (2(ad+bc) - 2ao)t^3
		intersection.d = 2f * ((basis.a.x * basis.d.x) + (basis.a.y * basis.d.y) + (basis.a.z * basis.d.z) + (basis.b.x * basis.c.x) + (basis.b.y * basis.c.y) + (basis.b.z * basis.c.z));
		// (2(bd+c^2) - 2bo)t^2
		intersection.e = (2f * ((basis.b.x * basis.d.x) + (basis.b.y * basis.d.y) + (basis.b.z * basis.d.z))) + (basis.c.x * basis.c.x) + (basis.c.y * basis.c.y) + (basis.c.z * basis.c.z) - (2f * ((basis.b.x * o.x) + (basis.b.y * o.y) + (basis.b.z * o.z)));
		// (2cd - 2co)t
		intersection.f = 2f * ((basis.c.x * basis.d.x) + (basis.c.y * basis.d.y) + (basis.c.z * basis.d.z)) - 2f * ((basis.c.x * o.x) + (basis.c.y * o.y) + (basis.c.z * o.z));
		// d^2 + o^2 - 2do[ - r^2]
		intersection.g = ((basis.d.x * basis.d.x) + (basis.d.y * basis.d.y) + (basis.d.z * basis.d.z) - 2f * ((basis.d.x * o.x) + (basis.d.y * o.y) + (basis.d.z * o.z)) + (o.x * o.x) + (o.y * o.y) + (o.z * o.z)) - (r * r);

		return intersection;
	}

	public Vector3 GetPosition(float t) {
		return basis.Solve(t);
	}

	public Vector3 GetTangent(float t) {
		return derivative.Solve(t);
	}

	public float Length {
		get { return _arcLength; }
	}

	public bool IsWithinArc(float s) {
		return s > _arcOffset && s <= _arcOffset + _arcLength;
	}

	public float GlobalToLocal(float s) {
		return s - _arcOffset;
	}

	public float LocalToGlobal(float s) {
		return s + _arcOffset;
	}

	public float ArcLength(float tmax) {
		float prev = GetTangent(0f).magnitude;

		float sum = 0f;
		for(int i = 1; i <= 10; i++) {
			float t = (i / 10f) * tmax;

			float f = GetTangent(t).magnitude;

			sum += tmax * (prev + f) / 10f;

			prev = f;
		}

		return  sum / 2f;
	}

	public Quaternion GetRotation(float t, Vector3 up) {
		Vector3 tangent = GetTangent(t);
		Vector3 binormal = Vector3.Cross(up, tangent).normalized;
		Vector3 normal = Vector3.Cross(tangent, binormal);

		if(tangent.magnitude <= 0.001f || tangent == Vector3.zero || normal.magnitude <= 0.001f || normal == Vector3.zero) {
			Debug.Log ("bad thing - tangent.magnitude = " + tangent.magnitude + ", normal.magnitude = " + normal.magnitude);
		}

		return Quaternion.LookRotation(tangent, normal);
	}

	public Point GetPoint(float t, int index, float len) {
		Vector3 position = GetPosition(t);
		Vector3 tangent = GetTangent(t);

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

	public float GetCurveParameter(float s) {
		float t = s / Length; // Initial guess

		float lower = 0f, upper = 1f;
		float MAX_ITERATIONS = 10;

		for(int i = 0; i < MAX_ITERATIONS; i++) {
			float f = ArcLength(t) - s;
			if(Mathf.Abs(f) < 0.001f) {
				return t;
			}

			float df = GetTangent(t).magnitude;

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

	public IEnumerable<Point> Sample(int points) {
		yield return GetPoint(0f, 0, 0f);

		for(int n = 1; n < points - 1; n++) {
			float s = (n / (points - 1f)) * Length;

			yield return GetPoint(GetCurveParameter(s), n, s);
		}

		yield return GetPoint(1f, points, Length);
	}


	public IEnumerable<Point> SampleUnparameterized(int len) {
		for(int n = 0; n < len; n++) {
			float t = (float)n / (len - 1);
			yield return GetPoint(t, n, 0f);
		}
	}
}
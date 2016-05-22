using UnityEngine;
using System.Collections;

public struct CatmullRomSpline {

	public struct CubicPoly {
		public float c0, c1, c2, c3;

		public float eval(float t) {
			float t2 = t*t;
			float t3 = t2 * t;
			return c0 + c1*t + c2*t2 + c3*t3;
		}
	}

	public struct Point {
		public Vector3 position;
		public Quaternion orientation;

		public Point (Vector3 position, Quaternion orientation) {
			this.position = position;
			this.orientation = orientation;
		}

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

	/*
 * Compute coefficients for a cubic polynomial
 *   p(s) = c0 + c1*s + c2*s^2 + c3*s^3
 * such that
 *   p(0) = x0, p(1) = x1
 *  and
 *   p'(0) = t0, p'(1) = t1.
 */
	void InitCubicPoly(float x0, float x1, float t0, float t1, CubicPoly p)
	{
		p.c0 = x0;
		p.c1 = t0;
		p.c2 = -3*x0 + 3*x1 - 2*t0 - t1;
		p.c3 = 2*x0 - 2*x1 + t0 + t1;
	}

	public float GetCubicPoly(float x0, float x1, float k0, float k1, float t)
	{
		float c0 = x0;
		float c1 = k0;
		float c2 = -3*x0 + 3*x1 - 2*k0 - k1;
		float c3 = 2*x0 - 2*x1 + k0 + k1;

		float t2 = t*t;
		float t3 = t2 * t;
		return c0 + c1*t + c2*t2 + c3*t3;
	}

	// standard Catmull-Rom spline: interpolate between x1 and x2 with previous/following points x0/x3
	// (we don't need this here, but it's for illustration)
	void InitCatmullRom(float x0, float x1, float x2, float x3, CubicPoly p)
	{
		// Catmull-Rom with tension 0.5
		InitCubicPoly(x1, x2, 0.5f*(x2-x0), 0.5f*(x3-x1), p);
	}

	// compute coefficients for a nonuniform Catmull-Rom spline
	void InitNonuniformCatmullRom(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2, CubicPoly p)
	{
		// compute tangents when parameterized in [t1,t2]
		float t1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
		float t2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

		// rescale tangents for parametrization in [0,1]
		t1 *= dt1;
		t2 *= dt1;

		InitCubicPoly(x1, x2, t1, t2, p);
	}

	public float GetNonuniformCatmullRom(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2, float t)
	{
		// compute tangents when parameterized in [t1,t2]
		float k1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
		float k2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

		// rescale tangents for parametrization in [0,1]
		k1 *= dt1;
		k2 *= dt1;

		float c0 = x1;
		float c1 = k1;
		float c2 = -3*x1 + 3*x2 - 2*k1 - k2;
		float c3 = 2*x1 - 2*x2 + k1 + k2;

		float t2 = t*t;
		float t3 = t2 * t;
		return c0 + c1*t + c2*t2 + c3*t3;
	}

	float VecDistSquared(Vector3 p, Vector3 q) {
		float dx = q.x - p.x;
		float dy = q.y - p.y;
		return dx*dx + dy*dy;
	}

	void InitCentripetalCR(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
		CubicPoly px, CubicPoly py) {
		float dt0 = Mathf.Pow(VecDistSquared(p0, p1), 0.25f);
		float dt1 = Mathf.Pow(VecDistSquared(p1, p2), 0.25f);
		float dt2 = Mathf.Pow(VecDistSquared(p2, p3), 0.25f);

		// safety check for repeated points
		if (dt1 < 1e-4f)    dt1 = 1.0f;
		if (dt0 < 1e-4f)    dt0 = dt1;
		if (dt2 < 1e-4f)    dt2 = dt1;

		InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2, px);
		InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2, py);
	}



	public Vector3 GetCentripetalCR(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float dt0 = Mathf.Pow(VecDistSquared(p0, p1), 0.25f);
		float dt1 = Mathf.Pow(VecDistSquared(p1, p2), 0.25f);
		float dt2 = Mathf.Pow(VecDistSquared(p2, p3), 0.25f);

		// safety check for repeated points
		if (dt1 < 1e-4f)    dt1 = 1.0f;
		if (dt0 < 1e-4f)    dt0 = dt1;
		if (dt2 < 1e-4f)    dt2 = dt1;

		//InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2, px);
		//InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2, py);

		Vector3 m1 = (p1 - p0) / dt0 - (p2 - p0) / (dt0 + dt1) + (p2 - p1) / dt1;
		Vector3 m2 = (p2 - p1) / dt1 - (p3 - p1) / (dt1 + dt2) + (p3 - p2) / dt2;

		//float k1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
		//float k2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

		// rescale tangents for parametrization in [0,1]
		m1 *= dt1;
		m2 *= dt1;

		Vector3 c0 = p1;
		Vector3 c1 = m1;
		Vector3 c2 = -3*p1 + 3*p2 - 2*m1 - m2;
		Vector3 c3 = 2*p1 - 2*p2 + m1 + m2;

		float t2 = t*t;
		float t3 = t2 * t;
		return c0 + c1*t + c2*t2 + c3*t3;

	}

	public Vector3 GetCentripetalCRTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		float dt0 = Mathf.Pow(VecDistSquared(p0, p1), 0.25f);
		float dt1 = Mathf.Pow(VecDistSquared(p1, p2), 0.25f);
		float dt2 = Mathf.Pow(VecDistSquared(p2, p3), 0.25f);

		// safety check for repeated points
		if (dt1 < 1e-4f)    dt1 = 1.0f;
		if (dt0 < 1e-4f)    dt0 = dt1;
		if (dt2 < 1e-4f)    dt2 = dt1;

		//InitNonuniformCatmullRom(p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2, px);
		//InitNonuniformCatmullRom(p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2, py);

		Vector3 m1 = (p1 - p0) / dt0 - (p2 - p0) / (dt0 + dt1) + (p2 - p1) / dt1;
		Vector3 m2 = (p2 - p1) / dt1 - (p3 - p1) / (dt1 + dt2) + (p3 - p2) / dt2;

		//float k1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
		//float k2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

		// rescale tangents for parametrization in [0,1]
		m1 *= dt1;
		m2 *= dt1;

		Vector3 c0 = m1;
		Vector3 c1 = -6*p1 + 6*p2 - 4*m1 - 2*m2;
		Vector3 c2 = 6*p1 - 6*p2 + 3*m1 + 3*m2;

		float t2 = t*t;
		return c0 + c1 * t + c2 * t2;

	}

	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;

	public Point GetPoint(float t) {
		float dt0 = Mathf.Pow(VecDistSquared(p0, p1), 0.25f);
		float dt1 = Mathf.Pow(VecDistSquared(p1, p2), 0.25f);
		float dt2 = Mathf.Pow(VecDistSquared(p2, p3), 0.25f);

		// safety check for repeated points
		if (dt1 < 1e-4f)    dt1 = 1.0f;
		if (dt0 < 1e-4f)    dt0 = dt1;
		if (dt2 < 1e-4f)    dt2 = dt1;

		Vector3 m1 = (p1 - p0) / dt0 - (p2 - p0) / (dt0 + dt1) + (p2 - p1) / dt1;
		Vector3 m2 = (p2 - p1) / dt1 - (p3 - p1) / (dt1 + dt2) + (p3 - p2) / dt2;

		m1 *= dt1;
		m2 *= dt1;

		Vector3 c0 = p1;
		Vector3 c1 = m1;
		Vector3 c2 = -3*p1 + 3*p2 - 2*m1 - m2;
		Vector3 c3 = 2*p1 - 2*p2 + m1 + m2;

		float t2 = t*t;
		float t3 = t2 * t;
		Vector3 position = c0 + c1*t + c2*t2 + c3*t3;

		Vector3 d0 = m1;
		Vector3 d1 = -6*p1 + 6*p2 - 4*m1 - 2*m2;
		Vector3 d2 = 6*p1 - 6*p2 + 3*m1 + 3*m2;

		Vector3 tangent = d0 + d1 * t + d2 * t2;

		Vector3 binormal = Vector3.Cross(Vector3.up, tangent).normalized;
		Vector3 normal = Vector3.Cross(tangent, binormal);
		Quaternion orientation = Quaternion.LookRotation(tangent, normal);

		return new Point (position, orientation);
	}

	public void Sample(Point[] points) {
		float len = points.Length - 1;
		for (int n = 0; n <= len; n++) {
			float t = (float)n / len;
			points [n] = GetPoint(t);
		}
	}
}

using UnityEngine;
using System.Collections.Generic;

public struct CatmullRomSpline {

	public struct Point {
		public Vector3 position;
		public Quaternion orientation;
		public int index;

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

	public Point GetPoint(float t) {
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

		Vector3 c0 = p1;
		Vector3 c1 = m1;
		Vector3 c2 = -3 * p1 + 3 * p2 - 2 * m1 - m2;
		Vector3 c3 = 2 * p1 - 2 * p2 + m1 + m2;

		float t2 = t * t;
		float t3 = t2 * t;
		Vector3 position = c0 + c1 * t + c2 * t2 + c3 * t3;

		Vector3 d0 = m1;
		Vector3 d1 = -6 * p1 + 6 * p2 - 4 * m1 - 2 * m2;
		Vector3 d2 = 6 * p1 - 6 * p2 + 3 * m1 + 3 * m2;

		Vector3 tangent = d0 + d1 * t + d2 * t2;

		Vector3 binormal = Vector3.Cross(Vector3.up, tangent).normalized;
		Vector3 normal = Vector3.Cross(tangent, binormal);
		Quaternion orientation = Quaternion.LookRotation(tangent, normal);

		Point p = new Point();
		p.position = position;
		p.orientation = orientation;
		return p;
	}

	public IEnumerable<Point> Sample(int len) {
		for(int n = 0; n < len; n++) {
			float t = (float)n / (len - 1);
			Point p = GetPoint(t);
			p.index = n;
			yield return p;
		}
	}
}

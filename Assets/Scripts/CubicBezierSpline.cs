using UnityEngine;
using System.Collections;

public struct CubicBezierSpline {

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

	public Vector3 a;
	public Vector3 b;
	public Vector3 c;
	public Vector3 d;

	public CubicBezierSpline (Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	/*
	Vector3 GetPosition(float t) {
		float oneMinusT = 1f - t;
		float oneMinusT2 = oneMinusT * oneMinusT;
		float t2 = t * t;
		return a * (oneMinusT2 * oneMinusT)
		+ b * (3f * oneMinusT2 * t)
		+ c * (3f * oneMinusT * t2)
		+ d * (t2 * t);
	}

	Vector3 GetTangent(float t) {
		float oneMinusT = 1f - t;
		float oneMinusT2 = oneMinusT * oneMinusT;
		float t2 = t * t;
		return (a * (-oneMinusT2)
		+ b * (3f * oneMinusT2 - 2f * oneMinusT)
		+ c * (-3f * t2 + 2f * t)
		+ d * (t2)).normalized;
	}

	Vector3 GetNormal(float t, Vector3 up) {
		Vector3 tangent = GetTangent(t);
		Vector3 binormal = Vector3.Cross(up, tangent).normalized;
		return Vector3.Cross(tangent, binormal);
	}

	Quaternion GetOrientation(float t, Vector3 up) {
		Vector3 tng = GetTangent(t);
		Vector3 nrm = GetNormal(t, up);
		return Quaternion.LookRotation(tng, nrm);
	}
*/

	public Point GetPoint(float t) {
		float oneMinusT = 1f - t;
		float oneMinusT2 = oneMinusT * oneMinusT;
		float t2 = t * t;

		Vector3 position = a * (oneMinusT2 * oneMinusT)
		                   + b * (3f * oneMinusT2 * t)
		                   + c * (3f * oneMinusT * t2)
		                   + d * (t2 * t);

		Vector3 tangent = a * (-oneMinusT2)
		                  + b * (3f * oneMinusT2 - 2f * oneMinusT)
		                  + c * (-3f * t2 + 2f * t)
		                  + d * (t2);

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

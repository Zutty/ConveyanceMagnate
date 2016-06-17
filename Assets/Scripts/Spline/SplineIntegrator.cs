using UnityEngine;
using System.Collections;

namespace Spline {
	public class SplineIntegrator : MonoBehaviour {

		private CompositeSpline _spline;

		public void Start() {
			_spline = GetComponent<CompositeSpline>();
		}

		public float Tmax {
			get { return (float)_spline.Length; }
		}

		public float ArcLength(float tmin, float tmax, int steps) {
			// Integrate numerically by Trapezium rule
			float prev = _spline.GetTangentContinuous(tmin).magnitude;

			float sum = 0f;
			for(int i = 1; i <= steps; i++) {
				float t = tmin + ((i / (float)steps) * (tmax - tmin));

				float f = _spline.GetTangentContinuous(t).magnitude;

				sum += (tmax - tmin) * (prev + f) / (float)steps;

				prev = f;
			}

			return  sum / 2f;
		}

		public float ArcLength(float t) {
			return ArcLength(0f, t, Mathf.RoundToInt(t * 10f));
		}

		public float ArcLength() {
			return ArcLength(Tmax);
		}

		public float CurveParameter(float s) {
			float t = s / ArcLength(); // Initial guess

			float lower = 0f, upper = Tmax;
			int MAX_ITERATIONS = 10;
			float lastf = 0f;

			for(int i = 0; i < MAX_ITERATIONS; i++) {
				float f = ArcLength(t) - s;
				lastf = f;
				if(Mathf.Abs(f) < 0.001f) {
					return t;
				}

				float df = _spline.GetTangentContinuous(t).magnitude;

				float tCandidate = t - (f / df);

				if(f > 0) {
					upper = t;
					t = (tCandidate <= lower) ? (upper + lower) / 2f : tCandidate;
				} else {
					lower = t;
					t = (tCandidate >= upper) ? (upper + lower) / 2f : tCandidate;
				}
			}

			Debug.LogWarning("Root for param " + s + " was not found after " + MAX_ITERATIONS + " iterations <"+lastf+"; "+lower+", "+t+", "+upper+">");
			return t;
		}

		public float CircleIntersection(Vector3 c, float r, float initial) {
			float t = initial;
			float MAX_ITERATIONS = 10;
			float lastf = 0f;

			for(int i = 0; i < MAX_ITERATIONS; i++) {
				Vector3 position = _spline.GetPositionContinuous(t);
				float f = (position - c).sqrMagnitude - (r * r);
				lastf = f;

				if(Mathf.Abs(f) < 0.001f) {
					return t;
				}

				Vector3 tangent = _spline.GetTangentContinuous(t);
				float df = 2f * (((position.x - c.x) * tangent.x) + ((position.y - c.y) * tangent.y) + ((position.z - c.z) * tangent.z));

				t -= f / df;

				if(t < 0f || t > Tmax) {
					return 0;
				}
			}

			Debug.LogWarning("Circle intersection [c = " + c + ", r = " + r + ", initial = " + initial + "] root was not found after " + MAX_ITERATIONS + " iterations <"+lastf+"; "+t+">");
			return t;
		}

		public Quaternion GetRotation(float t, Vector3 up) {
			Vector3 tangent = _spline.GetTangentContinuous(t);
			Vector3 binormal = Vector3.Cross(up, tangent).normalized;
			Vector3 normal = Vector3.Cross(tangent, binormal);

			if(tangent.magnitude <= 0.001f || tangent == Vector3.zero || normal.magnitude <= 0.001f || normal == Vector3.zero) {
				Debug.Log ("bad thing - tangent.magnitude = " + tangent.magnitude + ", normal.magnitude = " + normal.magnitude);
			}

			return Quaternion.LookRotation(tangent, normal);
		}

		public void UpdateTransform(float s, Transform transform) {
			if(s < 0f) {
				return;
			}

			CatmullRomSpline spline = _spline.CurveAtArcLength(s);

			float t = spline.GetCurveParameter(s);
			transform.position = spline.GetPosition(t);
			transform.rotation = spline.GetRotation(t, Vector3.up);
		}

		public void UpdateTransformTrailing(float s, float trail, Transform transform) {
			if(s < 0f || s > ArcLength()) {
				return;
			}

			float t = 0f;//CircleIntersection(_spline.GetPositionContinuous(CurveParameter(sc)), r, (sc - r) / ArcLength());

			transform.position = _spline.GetPositionContinuous(t);
			transform.rotation = GetRotation(t, Vector3.up);
		}

	}
}

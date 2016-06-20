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

		public float ArcLength(float t) {
			return _spline.CurveAtParameter(t).ArcLength(t);
		}

		public float CurveParameter(float s) {
			float t = s / _spline.ArcLength;

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

		public float CircleIntersection(Vector3 c, float r, float lower, float upper) {
			float t = (lower + upper) / 2f;
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

			Debug.LogWarning("Circle intersection [c = " + c + ", r = " + r + ", lower = " + lower + ", upper = " + upper + "] root was not found after " + MAX_ITERATIONS + " iterations <"+lastf+"; "+t+">");
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

		public SplinePoint GetPoint(float s) {
			s = Mathf.Clamp(s, 0f, _spline.ArcLength - 1f);

			CatmullRomSpline spline = _spline.CurveAtArcLength(s);

			float t = spline.GetCurveParameter(spline.GlobalToLocal(s));

			SplinePoint p = new SplinePoint();
			p.position = spline.GetPosition(t);
			p.rotation = spline.GetRotation(t, Vector3.up);
			return p;
		}

		public SplinePoint GetPointTrailing(float s, Vector3 c, float trail) {
			s = Mathf.Clamp(s, 0f, _spline.ArcLength);

			float MINIMUM_CURVE_RADIUS = 30f;
			float minArc = 2f * MINIMUM_CURVE_RADIUS * Mathf.Asin(trail / (2f * MINIMUM_CURVE_RADIUS));

			float lowerBound = _spline.GetCurveParameter(s - minArc);
			float upperBound = _spline.GetCurveParameter(s - trail);

			float t = CircleIntersection(c, trail, lowerBound, upperBound);

			SplinePoint p = new SplinePoint();
			p.position = _spline.GetPositionContinuous(t);
			p.rotation = GetRotation(t, Vector3.up);
			return p;
		}
	}
}

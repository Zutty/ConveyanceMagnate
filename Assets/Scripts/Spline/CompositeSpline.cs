using UnityEngine;
using System.Collections.Generic;
using Maths;

namespace Spline {
	public class CompositeSpline : MonoBehaviour {
		
		public List<Transform> points;

		private List<CubicSpline> _curves = new List<CubicSpline>();
		private float _arcLength;

		public int Length {
			get { return _curves.Count; }
		}

		public float ArcLength {
			get { return _arcLength; }
		}

		public void Start() {
			RecalculateCurves();
		}

		public CubicSpline this[int index] {
			get { return _curves[index]; }
		}

		public void Update() {
			bool recalculate = false;

			for(int i = 0; i < points.Count; i++) {
				if(points[i].hasChanged) {
					points[i].hasChanged = false;
					recalculate = true;
				}
			}

			if(recalculate) {
				RecalculateCurves();
			}
		}

		public void AddPoint(Transform point, int index) {
			points.Insert(index, point);
			RecalculateCurves();
		}

		private void RecalculateCurves() {
			_curves.Clear();

			float len = 0;
			for(int offset = 0; offset < points.Count - 3; offset++) {
				HermiteForm control = new HermiteForm();
				control.p0 = points[offset].position;
				control.p1 = points[offset + 1].position;
				control.m0 = points[offset + 2].position;
				control.m1 = points[offset + 3].position;
				CubicSpline spline = Splines.HermiteSpline(control);
				_curves.Add(spline);

				len += spline.arcLength;
			}
			_arcLength = len;
		}

		public CubicSpline CurveAtParameter(float t) {
			CheckT(t);
			return _curves[(int)Mathf.Clamp(Mathf.Floor(t), 0, _curves.Count - 1)];
		}

		public CubicSpline CurveAtArcLength(float s) {
			CheckS(s);
			float acc = 0;
			for(int i = 0; i < _curves.Count; i++) {
				acc += _curves[i].arcLength;
				if(s <= acc) {
					return _curves[i];
				}
			}
			throw new UnityException("s " + s + " not valid");
		}

		public float GetCurveParameter(float s) {
			CheckS(s);
			float acc = 0;
			for(int i = 0; i < _curves.Count; i++) {
				if(s <= acc + _curves[i].arcLength) {
					return (float)i + Splines.GetCurveParameter(_curves[i], s - acc);
				}
				acc += _curves[i].arcLength;
			}
			throw new UnityException("s " + s + " not valid");
		}

		public Vector3 GetPositionContinuous(float t) {
			CheckT(t);
			return CurveAtParameter(t).basis.Solve(t - Mathf.Floor(t));
		}

		public Vector3 GetTangentContinuous(float t) {
			CheckT(t);
			return CurveAtParameter(t).derivative.Solve(t - Mathf.Floor(t));
		}

		private void CheckT(float t) {
			if(t < 0f || t > (float)Length) {
				throw new UnityException("t " + t + " is not within parameter bounds");
			}
		}

		private void CheckS(float s) {
			if(s < 0f || s > ArcLength) {
				throw new UnityException("s " + s + " is not within arc length");
			}
		}
	}
}

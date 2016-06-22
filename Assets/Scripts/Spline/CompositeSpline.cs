﻿using UnityEngine;
using System.Collections.Generic;

namespace Spline {
	public class CompositeSpline : MonoBehaviour {
		
		public List<Transform> points;

		private List<CatmullRomSpline> _curves = new List<CatmullRomSpline>();
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

		private void RecalculateCurves() {
			_curves.Clear();

			float len = 0;
			for(int offset = 0; offset < points.Count - 3; offset++) {
				CatmullRomSpline spline = new CatmullRomSpline();
				spline.p0 = points[offset].position;
				spline.p1 = points[offset + 1].position;
				spline.p2 = points[offset + 2].position;
				spline.p3 = points[offset + 3].position;
				spline.CalculateBasis(len);
				_curves.Add(spline);

				len += spline.Length;
			}
			_arcLength = len;
		}

		public CatmullRomSpline CurveAtParameter(float t) {
			CheckT(t);
			return _curves[(int)Mathf.Clamp(Mathf.Floor(t), 0, _curves.Count - 1)];
		}

		public CatmullRomSpline CurveAtArcLength(float s) {
			CheckS(s);
			for(int i = 0; i < _curves.Count; i++) {
				if(_curves[i].IsWithinArc(s)) {
					return _curves[i];
				}
			}
			throw new UnityException("s " + s + " not valid");
		}

		public float GetCurveParameter(float s) {
			CheckS(s);

			for(int i = 0; i < _curves.Count; i++) {
				if(_curves[i].IsWithinArc(s)) {
					return (float)i + _curves[i].GetCurveParameter(_curves[i].GlobalToLocal(s));
				}
			}
			throw new UnityException("s " + s + " not valid");
		}

		public Vector3 GetPositionContinuous(float t) {
			CheckT(t);
			return CurveAtParameter(t).GetPosition(t - Mathf.Floor(t));
		}

		public Vector3 GetTangentContinuous(float t) {
			CheckT(t);
			return CurveAtParameter(t).GetTangent(t - Mathf.Floor(t));
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

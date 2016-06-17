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
			Debug.Log("Recalculating...");
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
			Debug.Log("  _curves.Count = "+_curves.Count);
		}

		public CatmullRomSpline CurveAtParameter(float t) {
			return _curves [(int)Mathf.Clamp (Mathf.Floor (t), 0, _curves.Count - 1)];
		}

		public CatmullRomSpline CurveAtArcLength(float s) {
			for(int i = 0; i < _curves.Count; i++) {
				if(_curves[i].IsWithinArc(s)) {
					return _curves[i];
				}
			}
			throw new UnityException();
		}

		public Vector3 GetPositionContinuous(float t) {
			return CurveAtParameter(t).GetPosition(t - Mathf.Floor(t));
		}

		public Vector3 GetTangentContinuous(float t) {
			return CurveAtParameter(t).GetTangent(t - Mathf.Floor(t));
		}
	}
}

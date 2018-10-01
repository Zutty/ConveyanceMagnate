using UnityEngine;
using System.Collections.Generic;

namespace Spline {
    public class CompositeSpline : MonoBehaviour {
        public List<Transform> points;

        private readonly List<CubicCurve> _curves = new List<CubicCurve>();

        public int Length {
            get { return _curves.Count; }
        }

        public float ArcLength { get; private set; }

        public void Start() {
            RecalculateCurves();
        }

        public CubicCurve this[int index] {
            get { return _curves[index]; }
        }

        public void Update() {
            var recalculate = false;

            foreach (var t in points) {
                if (!t.hasChanged) continue;
                t.hasChanged = false;
                recalculate = true;
            }

            if (recalculate) {
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
            for (var offset = 0; offset < points.Count - 3; offset++) {
                HermiteForm control;
                control.p0 = points[offset].position;
                control.p1 = points[offset + 1].position;
                control.m0 = points[offset + 2].position;
                control.m1 = points[offset + 3].position;
                var spline = Splines.HermiteSpline(control);
                _curves.Add(spline);

                len += spline.arcLength;
            }

            ArcLength = len;
        }

        public CubicCurve CurveAtParameter(float t) {
            CheckT(t);
            return _curves[(int) Mathf.Clamp(Mathf.Floor(t), 0, _curves.Count - 1)];
        }

        public CubicCurve CurveAtArcLength(float s) {
            CheckS(s);
            float acc = 0;
            for (int i = 0; i < _curves.Count; i++) {
                acc += _curves[i].arcLength;
                if (s <= acc) {
                    return _curves[i];
                }
            }

            throw new UnityException("s " + s + " not valid");
        }

        public float GetCurveParameter(float s) {
            CheckS(s);
            float acc = 0;
            for (int i = 0; i < _curves.Count; i++) {
                if (s <= acc + _curves[i].arcLength) {
                    return i + Splines.GetCurveParameter(_curves[i], s - acc);
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
            if (t < 0f || t > Length) {
                throw new UnityException("t " + t + " is not within parameter bounds");
            }
        }

        private void CheckS(float s) {
            if (s < 0f || s > ArcLength) {
                throw new UnityException("s " + s + " is not within arc length");
            }
        }
    }
}
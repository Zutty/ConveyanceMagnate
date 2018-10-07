using UnityEngine;

namespace Spline {
    public class ContinuousCurve : MonoBehaviour {

        private SplineGraph _splineGraph;

        private void Start() {
            _splineGraph = GetComponent<SplineGraph>();
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
        
        public float CircleIntersection(Vector3 c, float r, float lower, float upper) {
            var t = (lower + upper) / 2f;
            const float MAX_ITERATIONS = 10;
            var lastf = 0f;

            for (var i = 0; i < MAX_ITERATIONS; i++) {
                var position = _spline.GetPositionContinuous(t);
                var f = (position - c).sqrMagnitude - (r * r);
                lastf = f;

                if (Mathf.Abs(f) < 0.001f) {
                    return t;
                }

                var tangent = _spline.GetTangentContinuous(t);
                var df = 2f * ((position.x - c.x) * tangent.x + (position.y - c.y) * tangent.y +
                               (position.z - c.z) * tangent.z);

                t -= f / df;

                if (t < 0f || t > _spline.Length) {
                    return 0;
                }
            }

            Debug.LogWarning("Circle intersection [c = " + c + ", r = " + r + ", lower = " + lower + ", upper = " +
                             upper + "] root was not found after " + MAX_ITERATIONS + " iterations <" + lastf + "; " +
                             t + ">");
            return t;
        }

        public Quaternion GetRotation(float t, Vector3 up) {
            Vector3 tangent = _spline.GetTangentContinuous(t);
            Vector3 binormal = Vector3.Cross(up, tangent).normalized;
            Vector3 normal = Vector3.Cross(tangent, binormal);

            if (tangent.magnitude <= 0.001f || tangent == Vector3.zero || normal.magnitude <= 0.001f ||
                normal == Vector3.zero) {
                Debug.Log("bad thing - tangent.magnitude = " + tangent.magnitude + ", normal.magnitude = " +
                          normal.magnitude);
            }

            return Quaternion.LookRotation(tangent, normal);
        }

        public SplinePoint GetPoint(float s) {
            s = Mathf.Clamp(s, 0f, _spline.ArcLength - 1f);

            var spline = _spline.CurveAtArcLength(s);

            var t = _spline.GetCurveParameter(s);

            SplinePoint p;
            p.position = spline.basis.Solve(t);

            var tangent = spline.derivative.Solve(t);
            var binormal = Vector3.Cross(Vector3.up, tangent).normalized;
            var normal = Vector3.Cross(tangent, binormal);

            if (tangent.magnitude <= 0.001f || tangent == Vector3.zero || normal.magnitude <= 0.001f ||
                normal == Vector3.zero) {
                Debug.Log("bad thing - tangent.magnitude = " + tangent.magnitude + ", normal.magnitude = " +
                          normal.magnitude);
            }

            p.rotation = Quaternion.LookRotation(tangent, normal);
            p.s = s;
            p.t = t;
            return p;
        }

        public SplinePoint GetPointTrailing(float s, Vector3 c, float trail) {
            const float MINIMUM_CURVE_RADIUS = 30f;
            var minArc = 2f * MINIMUM_CURVE_RADIUS * Mathf.Asin(trail / (2f * MINIMUM_CURVE_RADIUS));

            s = Mathf.Clamp(s, minArc, _spline.ArcLength);

            var lowerBound = GetCurveParameter(s - minArc);
            var upperBound = GetCurveParameter(s - trail);

            var t = CircleIntersection(c, trail, lowerBound, upperBound);

            SplinePoint p;
            p.position = _spline.GetPositionContinuous(t);
            p.rotation = GetRotation(t, Vector3.up);
            p.s = s;
            p.t = t;
            return p;
        }

        public Position Move(Position position, float distance) {
            position.s += distance;
            var curve = _splineGraph.GetCurve(position);
            if (position.s > curve.arcLength) {
                position.s - curve.arcLength;
            }

            return position;
        }

        public Vector3 GetPosition(Position position) {
            var curve = _splineGraph.GetCurve(position.a, position.b);
            return curve.GetPosition(position.s);
        }

        public Quaternion GetRotation(Position position) {
            return Quaternion.LookRotation();
        }
    }
}
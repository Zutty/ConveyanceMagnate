using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spline {
    public class ContinuousCurve : MonoBehaviour {

        public Path origin;

        public float Advance(float s) {
            float remainder;
            int steps;
            origin = WalkByArcLength(s, out remainder, out steps);
            return remainder;
        }

        public CubicCurve CurveAtParameter(float t) {
            return origin.Walk((int) Mathf.Floor(t)).GetCurve();
        }

        public CubicCurve CurveAtArcLength(float s) {
            float remainder;
            int steps;
            return WalkByArcLength(s, out remainder, out steps).GetCurve();
        }

        public float GetCurveParameter(float s) {
            float remainder;
            int steps;
            var path = WalkByArcLength(s, out remainder, out steps);
            return Splines.GetCurveParameter(path.GetCurve(), remainder) + steps;
        }

        private Path WalkByArcLength(float arcLen, out float s, out int steps) {
            var walk = origin;
            s = arcLen;
            steps = 0;
            
            while (s < 0 || s >= walk.GetCurve().arcLength) {
                if (s >= walk.GetCurve().arcLength) {
                    s -= walk.GetCurve().arcLength;
                    steps++;
                    walk = walk.WalkForward();
                    continue;
                }

                steps--;
                walk = walk.WalkBack();
                s += walk.GetCurve().arcLength;
            }
            return walk;

        }
        
        public float ArcLength(float t) {
            var walk = origin;
            var s = 0f;
            
            while (t < 0f || t >= 1f) {
                if (t >= 1f) {
                    s += walk.GetCurve().arcLength;
                    t--;
                    walk = walk.WalkForward();
                    continue;
                }

                walk = walk.WalkBack();
                s -= walk.GetCurve().arcLength;
                t++;
            }
            
            return s + Splines.ArcLength(walk.GetCurve().derivative, t);
        }

        public Vector3 GetPositionContinuous(float t) {
            return CurveAtParameter(t).GetPosition(t - Mathf.Floor(t));
        }

        public Vector3 GetTangentContinuous(float t) {
            return CurveAtParameter(t).GetTangent(t - Mathf.Floor(t));
        }

        public float CircleIntersection(Vector3 c, float r, float lower, float upper) {
            var t = (lower + upper) / 2f;
            const float MAX_ITERATIONS = 10;
            var lastf = 0f;

            for (var i = 0; i < MAX_ITERATIONS; i++) {
                var position = GetPositionContinuous(t);
                var f = (position - c).sqrMagnitude - (r * r);
                lastf = f;

                if (Mathf.Abs(f) < 0.001f) {
                    return t;
                }

                var tangent = GetTangentContinuous(t);
                var df = 2f * ((position.x - c.x) * tangent.x + (position.y - c.y) * tangent.y +
                               (position.z - c.z) * tangent.z);

                t -= f / df;

                //if (t < 0f || t > 1) {
                //    return 0;
                //}
            }

            Debug.LogWarning("Circle intersection [c = " + c + ", r = " + r + ", lower = " + lower + ", upper = " +
                             upper + "] root was not found after " + MAX_ITERATIONS + " iterations <" + lastf + "; " +
                             t + ">");
            return t;
        }

        public SplinePoint GetPoint(float s) {
            float remainder;
            int steps;
            var curve = WalkByArcLength(s, out remainder, out steps).GetCurve();
            var t = Splines.GetCurveParameter(curve, remainder);

            return new SplinePoint {
                position = curve.GetPosition(t),
                rotation = Quaternion.LookRotation(curve.GetTangent(t)),
                s = s,
                t = t
            };
        }

        public SplinePoint GetPointTrailing(float s, Vector3 c, float trail) {
            const float MINIMUM_CURVE_RADIUS = 30f;
            var minArc = 2f * MINIMUM_CURVE_RADIUS * Mathf.Asin(trail / (2f * MINIMUM_CURVE_RADIUS));

            //s = Mathf.Clamp(s, minArc, _spline.ArcLength);

            var lowerBound = GetCurveParameter(s - minArc);
            var upperBound = GetCurveParameter(s - trail);

            var t = CircleIntersection(c, trail, lowerBound, upperBound);

            return new SplinePoint {
                position = GetPositionContinuous(t),
                rotation = Quaternion.LookRotation(GetTangentContinuous(t)),
                s = ArcLength(t),
                t = t
            };
        }
    }
}
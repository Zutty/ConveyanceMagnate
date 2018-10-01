﻿using UnityEngine;
using System.Collections.Generic;
using Maths;

namespace Spline {
    public static class Splines {
        public static CubicSpline HermiteSpline(HermiteForm control) {
            CubicPolynomial3 basis;
            basis.a = 2 * control.p0 - 2 * control.p1 + control.m0 + control.m1;
            basis.b = -3 * control.p0 + 3 * control.p1 - 2 * control.m0 - control.m1;
            basis.c = control.m0;
            basis.d = control.p0;

            QuadraticPolynomial3 derivative;
            derivative.a = 6 * control.p0 - 6 * control.p1 + 3 * control.m0 + 3 * control.m1;
            derivative.b = -6 * control.p0 + 6 * control.p1 - 4 * control.m0 - 2 * control.m1;
            derivative.c = control.m0;

            return new CubicSpline(basis, derivative, ArcLength(derivative));
        }

        public static void SubdivideHermiteSpline(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1,
            out CubicPolynomial3 m1Prime) {
            m1Prime.a = -12f * p0 - 6f * p1 - 3f * m0 + 3f * m1;
            m1Prime.b = 12f * p1 + 8f * m0 - 2f * m1;
            m1Prime.c = -48f * p0 - 5f * m0;
            m1Prime.d = 6f * p0;
        }

        public static CubicSpline CentripetalCatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var dt0 = Mathf.Sqrt(Vector3.Distance(p0, p1));
            var dt1 = Mathf.Sqrt(Vector3.Distance(p1, p2));
            var dt2 = Mathf.Sqrt(Vector3.Distance(p2, p3));

            // safety check for repeated points
            if (dt1 < Mathf.Epsilon) {
                dt1 = 1.0f;
            }

            if (dt0 < Mathf.Epsilon) {
                dt0 = dt1;
            }

            if (dt2 < Mathf.Epsilon) {
                dt2 = dt1;
            }

            var m1 = (p1 - p0) / dt0 - (p2 - p0) / (dt0 + dt1) + (p2 - p1) / dt1;
            var m2 = (p2 - p1) / dt1 - (p3 - p1) / (dt1 + dt2) + (p3 - p2) / dt2;

            m1 *= dt1;
            m2 *= dt1;

            CubicPolynomial3 basis;
            basis.a = 2 * p1 - 2 * p2 + m1 + m2;
            basis.b = -3 * p1 + 3 * p2 - 2 * m1 - m2;
            basis.c = m1;
            basis.d = p1;

            QuadraticPolynomial3 derivative;
            derivative.a = 6 * p1 - 6 * p2 + 3 * m1 + 3 * m2;
            derivative.b = -6 * p1 + 6 * p2 - 4 * m1 - 2 * m2;
            derivative.c = m1;

            //_arcLength = ArcLength(1f);
            //_arcOffset = arcOffset;

            return new CubicSpline();
        }

        public static float ArcLength(QuadraticPolynomial3 derivative, float tmax = 1f) {
            var prev = derivative.Solve(tmax).magnitude;

            var sum = 0f;
            for (var i = 1; i <= 10; i++) {
                var t = (i / 10f) * tmax;

                var f = derivative.Solve(t).magnitude;

                sum += tmax * (prev + f) / 10f;

                prev = f;
            }

            return sum / 2f;
        }

        public static float GetCurveParameter(CubicSpline spline, float s) {
            var t = s / spline.arcLength; // Initial guess

            float lower = 0f, upper = 1f;
            const float MAX_ITERATIONS = 10;

            for (var i = 0; i < MAX_ITERATIONS; i++) {
                var f = ArcLength(spline.derivative, t) - s;
                if (Mathf.Abs(f) < 0.001f) {
                    return t;
                }

                var df = spline.derivative.Solve(t).magnitude;

                var tCandidate = t - (f / df);

                if (f > 0) {
                    upper = t;
                    t = (tCandidate <= lower) ? (upper + lower) / 2f : tCandidate;
                } else {
                    lower = t;
                    t = (tCandidate >= upper) ? (upper + lower) / 2f : tCandidate;
                }
            }

            Debug.LogWarning("Root was not found after " + MAX_ITERATIONS + " iterations");
            return t;
        }

        // TODO encapsulate this in a Look-Up-Table

        public struct Point {
            public Vector3 position;
            public Quaternion orientation;
            public float len;
        }

        private static Point GetPoint(CubicSpline spline, float t, float len) {
            return new Point {
                position = spline.GetPosition(t),
                orientation = Quaternion.LookRotation(spline.GetTangent(t)),
                len = len
            };
        }

        public static IEnumerable<Point> Sample(CubicSpline spline, int points) {
            yield return GetPoint(spline, 0f, 0f);

            for (int n = 1; n < points - 1; n++) {
                float s = (n / (points - 1f)) * spline.arcLength;

                yield return GetPoint(spline, GetCurveParameter(spline, s), s);
            }

            yield return GetPoint(spline, 1f, spline.arcLength);
        }
    }
}
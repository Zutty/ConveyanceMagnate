using UnityEngine;
using Maths;

namespace Spline {
    public struct CubicSpline {
        public readonly CubicPolynomial3 basis;
        public readonly QuadraticPolynomial3 derivative;
        public readonly float arcLength;

        public CubicSpline(CubicPolynomial3 basis, QuadraticPolynomial3 derivative, float arcLength) {
            this.basis = basis;
            this.derivative = derivative;
            this.arcLength = arcLength;
        }

        public Vector3 GetPosition(float t) {
            return basis.Solve(t);
        }

        public Vector3 GetTangent(float t) {
            return derivative.Solve(t);
        }
    }
}
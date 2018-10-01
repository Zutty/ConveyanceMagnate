using UnityEngine;

namespace Maths {
    public struct QuadraticPolynomial3 {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 Solve(float x) {
            return (a * x * x) + (b * x) + c;
        }
    }
}
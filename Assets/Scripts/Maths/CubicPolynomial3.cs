using UnityEngine;
using System.Collections;

namespace Maths {
	public struct CubicPolynomial3 {

		public Vector3 a;
		public Vector3 b;
		public Vector3 c;
		public Vector3 d;

		public Vector3 Solve(float x) {
			return (a * x * x * x) + (b * x * x) + (c * x) + d;
		}
	}
}

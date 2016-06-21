using UnityEngine;
using System.Collections;

namespace Maths {
	public struct SexticPolynomialf {

		public float a;
		public float b;
		public float c;
		public float d;
		public float e;
		public float f;
		public float g;

		public float Solve(float x) {
			float x3 = x * x * x;
			return (a * x3 * x3) + (b * x3 * x * x) + (c * x3 * x) + (d * x3) + (e * x * x) + (f * x) + g;
		}
	}
}

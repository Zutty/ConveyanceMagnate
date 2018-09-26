using UnityEngine;
using System.Collections;
using Maths;

namespace Spline {
	public class SplineRenderer : MonoBehaviour {

		public ControlPoint a;
		public ControlPoint b;

		public CubicSpline GetSpline() {
			var hermiteForm = new HermiteForm();
			hermiteForm.p0 = a.position;
			hermiteForm.m0 = a.forwardTangent;
			hermiteForm.p1 = b.position;
			hermiteForm.m1 = b.backTangent;
			return Splines.HermiteSpline(hermiteForm);
		}

		public void OnDrawGizmos() {
			Gizmos.color = Color.blue;

			CubicSpline spline = GetSpline();

			Vector3 prev = a.position;

			for(float t = 0.05f; t < 1f; t += 0.05f) {
				Vector3 p = spline.basis.Solve(t);
				Gizmos.DrawLine(prev, p);
				prev = p;
			}

			Gizmos.DrawLine(prev, b.position);
		}
	}
}
using UnityEngine;
using System.Collections;
using Spline;
using Maths;

public class SubdivideTest : MonoBehaviour {

	[Range(0f, 1f)]
	public float u;

	public void OnDrawGizmos() {
		Gizmos.color = Color.red;

		var controlEdge = GetComponent<ControlEdge>();
		var originalCurve = controlEdge.curve;

		HermiteForm hermiteForm;
		hermiteForm.p0 = controlEdge.a.position;
		hermiteForm.m0 = controlEdge.a.forwardTangent;
		hermiteForm.p1 = controlEdge.b.position;
		hermiteForm.m1 = controlEdge.b.backTangent;
		var subdivided = Splines.HermiteSpline(Splines.Subdivide(hermiteForm, originalCurve, u));

		Vector3 prev = controlEdge.a.position;

		for(float t = 0.1f; t < 1f; t += 0.1f) {
			Vector3 p = subdivided.basis.Solve(t);
			Gizmos.DrawLine(prev, p);
			prev = p;
		}

		Gizmos.DrawLine(prev, originalCurve.basis.Solve(u));
	}
}

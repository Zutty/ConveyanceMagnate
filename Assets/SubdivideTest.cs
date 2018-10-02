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

		HermiteForm subdiv;
		subdiv.p0 = controlEdge.a.position;
		subdiv.m0 = u * controlEdge.a.forwardTangent;
		subdiv.p1 = originalCurve.basis.Solve(u);
		subdiv.m1 = u * originalCurve.derivative.Solve(u);
		var subdivided = Splines.HermiteSpline(subdiv);

		Vector3 prev = controlEdge.a.position;

		for(float t = 0.1f; t < 1f; t += 0.1f) {
			Vector3 p = subdivided.basis.Solve(t);
			Gizmos.DrawLine(prev, p);
			prev = p;
		}

		Gizmos.DrawLine(prev, originalCurve.basis.Solve(u));
	}
}

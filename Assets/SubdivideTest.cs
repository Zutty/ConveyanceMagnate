using UnityEngine;
using System.Collections;
using Spline;
using Maths;

public class SubdivideTest : MonoBehaviour {

	public Transform start;
	public Transform end;
	[Range(0f, 1f)]
	public float u;

	public void OnDrawGizmos() {
		Gizmos.color = Color.red;
/*
		CubicPolynomial3 originalBasis = new CubicPolynomial3();
		QuadraticPolynomial3 originalDerivative = new QuadraticPolynomial3();
		Splines.HermiteSpline(start.position, end.position, start.tang, end.tang, out originalBasis, out originalDerivative);

		CubicPolynomial3 m1Prime = new CubicPolynomial3();
		Splines.SubdivideHermiteSpline(start.position, end.position, start.tang, end.tang, out m1Prime);

		Vector3 q0 = start.position;
		Vector3 q1 = start.position;// - (start.tang / 3f);
		Vector3 q2 = end.position;// + (end.tang / 3f);
		Vector3 q3 = end.position;

		float u2 = u * u;
		float u3 = u2 * u;
		float omu = 1f - u;
		float omu2 = omu * omu;
		float omu3 = omu * omu;

		Vector3 qPrime2 = -omu2 * q0 + (2f * u * omu) * q1 + u2 * q2;
		Vector3 qPrime3 = omu3 * q0 - (3f * u * omu2) * q1 + (3f * u2 * omu) * q2 + u3 * q3;

		Vector3 mPrime1 = -3f * qPrime2 + 3f * qPrime3;

		CubicPolynomial3 basis = new CubicPolynomial3();
		QuadraticPolynomial3 derivative = new QuadraticPolynomial3();
		Splines.HermiteSpline(start.position, originalBasis.Solve(u), u * start.tang, u * originalDerivative.Solve(u));

		Vector3 prev = start.position;

		for(float t = 0.1f; t < 1f; t += 0.1f) {
			Vector3 p = basis.Solve(t);
			Gizmos.DrawLine(prev, p);
			prev = p;
		}

		Gizmos.DrawLine(prev, originalBasis.Solve(u));
		*/
	}
}

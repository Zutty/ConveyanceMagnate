using UnityEngine;
using System.Collections.Generic;

public class EditableSpline : MonoBehaviour {

	public List<Transform> points;

	public void UpdateTransform(float distance, Transform transform) {
		CatmullRomSpline spline = new CatmullRomSpline ();
		float s = distance;
		float len = 0;

		for(int offset = 0; offset < points.Count - 3; offset++) {
			s -= len;

			spline.p0 = points [offset].position;
			spline.p1 = points [offset + 1].position;
			spline.p2 = points [offset + 2].position;
			spline.p3 = points [offset + 3].position;

			len = spline.ArcLength(1f);

			if(s < len) {
				break;
			}
		}

		if(s > len) {
			return;
		}

		CatmullRomSpline.Point p = spline.GetPoint (spline.GetCurveParameter (s));

		transform.position = p.position;
		transform.rotation = p.orientation;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.white;

		foreach(Transform child in transform) {
			Gizmos.DrawWireSphere(child.position, 0.3f);
		}
	}
}

using UnityEngine;
using System.Collections;
using Spline;

public class SplineSelector : MonoBehaviour {

	public void Update () {
		if(Input.GetMouseButtonDown(2)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000) && hit.collider.gameObject.CompareTag("spline")) {
				Transform segment = hit.collider.transform.parent;
				EditableSpline editor = segment.parent.GetComponent<EditableSpline>();
				CompositeSpline spline = segment.parent.GetComponent<CompositeSpline>();

				int offset = spline.points.IndexOf(segment);
				editor.AddSection(offset + 1);
			}
		}
	}
}

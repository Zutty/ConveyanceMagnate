using UnityEngine;
using System.Collections;

public class SplineSelector : MonoBehaviour {

	public void Update () {
		if(Input.GetMouseButtonDown(2)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000) && hit.collider.gameObject.CompareTag("spline")) {
				Transform segment = hit.collider.transform.parent;
				EditableSpline spline = segment.parent.GetComponent<EditableSpline>();
				int offset = spline.points.IndexOf(segment);
				spline.AddSection(offset + 1);
			}
		}
	}
}

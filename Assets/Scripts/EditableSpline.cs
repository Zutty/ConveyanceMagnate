using UnityEngine;
using System.Collections.Generic;

public class EditableSpline : MonoBehaviour {

	public List<Transform> points;
	public GameObject sectionPrefab;

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

	public void OnDrawGizmos() {
		Gizmos.color = Color.white;

		foreach(Transform child in transform) {
			Gizmos.DrawWireSphere(child.position, 0.3f);
		}
	}

	public void Update() {
		if(Input.GetMouseButton(2)) {
			//GetComponentInParent<EditableSpline>().AddSection();
			AddSection(3);
		}
	}

	public void AddSection(int offset) {
		if(offset < 1 || offset >= points.Count - 2) {
			throw new UnityException();
		}

		Vector3 position = (points[offset].position + points[offset - 1].position) / 2;
		Quaternion rotation = Quaternion.LookRotation(points[offset].position - points[offset - 1].position);

		GameObject newSection = (GameObject)Instantiate(sectionPrefab, position, rotation);
		newSection.transform.parent = transform;

		points.Insert(offset, newSection.transform);

		for(int i = Mathf.Max(1, offset - 2); i <= offset + 1; i++) {
			Extrude extrude = points[i].GetComponent<Extrude>();
			extrude.a = points[i - 1];
			extrude.b = points[i];
			extrude.c = points[i + 1];
			extrude.d = points[i + 2];
		}

		if(offset < 3) {
			FollowTangent followTangent = points[0].GetComponent<FollowTangent>();
			followTangent.target = points[1];
			followTangent.tangent = points[2];
		}
		if(offset >= points.Count - 4) {
			FollowTangent followTangent = points[points.Count - 1].GetComponent<FollowTangent>();
			followTangent.target = points[points.Count - 2];
			followTangent.tangent = points[points.Count - 3];
		}
	}

	public void AddSectionAtEnd() {
		AddSection(points.Count - 2);
	}
}

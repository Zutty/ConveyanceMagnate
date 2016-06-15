using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditableSpline : MonoBehaviour, IEnumerable<CatmullRomSpline> {

	public List<Transform> points;
	public GameObject sectionPrefab;

	public CatmullRomSpline this[int offset] {
		get { 
			CatmullRomSpline spline = new CatmullRomSpline();
			spline.p0 = points[offset].position;
			spline.p1 = points[offset + 1].position;
			spline.p2 = points[offset + 2].position;
			spline.p3 = points[offset + 3].position;
			spline.CalculateBasis();
			return spline;
		}
	}

	public int Length {
		get { return points.Count - 3; }
	}

	public IEnumerator<CatmullRomSpline> GetEnumerator() {
		for(int offset = 0; offset < this.Length; offset++) {
			yield return this[offset];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public void UpdateTransform(float distance, Transform transform) {
		float s = distance;
		float len = 0;
		int offset;

		for(offset = 0; offset < Length; offset++) {
			s -= len;

			len = this[offset].ArcLength(1f);

			if(s < len) {
				break;
			}
		}

		if(s > len) {
			return;
		}

		CatmullRomSpline spline = this[offset];
		CatmullRomSpline.Point p = spline.GetPoint(spline.GetCurveParameter(s));

		transform.position = p.position;
		transform.rotation = p.orientation;
	}

	public void UpdateTransformTrailing(float distance, float trail, Transform transform) {
		float s = distance;
		float len = 0;
		int offset;

		for(offset = 0; offset < Length; offset++) {
			s -= len;

			len = this[offset].ArcLength(1f);

			if(s < len) {
				break;
			}
		}

		if(s > len) {
			return;
		}

		CatmullRomSpline spline = this[offset];

		CatmullRomSpline.Point p = spline.GetPoint(spline.GetCurveParameter(s));

		CatmullRomSpline.Point trailing = spline.GetPoint(spline.CircleIntersection(p.position, trail, spline.CircleIntersectionGuess(s, trail)));

		transform.position = trailing.position;
		transform.rotation = trailing.orientation;
	}

	public void OnDrawGizmos() {
		Gizmos.color = Color.white;

		foreach(Transform child in transform) {
			Gizmos.DrawWireSphere(child.position, 0.3f);
		}
	}

	public void AddSection(int offset) {
		if(offset < 1 || offset > points.Count - 2) {
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

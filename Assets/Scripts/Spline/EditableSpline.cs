using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Spline {
	public class EditableSpline : MonoBehaviour {

		public GameObject sectionPrefab;

		private CompositeSpline _spline;

		public void Start() {
			_spline = GetComponent<CompositeSpline>();
		}

		public void OnDrawGizmos() {
			Gizmos.color = Color.white;

			foreach(Transform child in transform) {
				Gizmos.DrawWireSphere(child.position, 0.3f);
			}
		}

		public void AddSection(int offset) {
			if(offset < 1 || offset > _spline.points.Count - 2) {
				throw new UnityException("Cant add section at offset " + offset);
			}

			Vector3 position = (_spline.points[offset].position + _spline.points[offset - 1].position) / 2;
			Quaternion rotation = Quaternion.LookRotation(_spline.points[offset].position - _spline.points[offset - 1].position);

			GameObject newSection = (GameObject)Instantiate(sectionPrefab, position, rotation);
			newSection.transform.parent = transform;

			_spline.points.Insert(offset, newSection.transform);

			for(int i = Mathf.Max(1, offset - 2); i <= Mathf.Min(_spline.points.Count - 3, offset + 1); i++) {
				Extrude extrude = _spline.points[i].GetComponent<Extrude>();
				extrude.a = _spline.points[i - 1];
				extrude.b = _spline.points[i];
				extrude.c = _spline.points[i + 1];
				extrude.d = _spline.points[i + 2];
			}

			if(offset < 3) {
				FollowTangent followTangent = _spline.points[0].GetComponent<FollowTangent>();
				followTangent.target = _spline.points[1];
				followTangent.tangent = _spline.points[2];
			}
			if(offset >= _spline.points.Count - 4) {
				FollowTangent followTangent = _spline.points[_spline.points.Count - 1].GetComponent<FollowTangent>();
				followTangent.target = _spline.points[_spline.points.Count - 2];
				followTangent.tangent = _spline.points[_spline.points.Count - 3];
			}
		}

		public void AddSectionAtEnd() {
			AddSection(_spline.points.Count - 2);
		}
	}
}

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

			Vector3 position = _spline[offset - 2].GetPosition(0.5f);//(_spline.points[offset].position + _spline.points[offset - 1].position) / 2;

			GameObject newSection = (GameObject)Instantiate(sectionPrefab, position, Quaternion.identity);
			newSection.transform.parent = transform;

			_spline.AddPoint(newSection.transform, offset);

			for(int i = 1; i < _spline.points.Count - 2; i++) {
				Extrude extrude = _spline.points[i].GetComponent<Extrude>();
				extrude.splineIndex = i - 1;
				_spline.points[i].name = "track_segment_" + i;
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

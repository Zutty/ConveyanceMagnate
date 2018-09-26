using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline {
	public class TangentHandle : MonoBehaviour {

		public enum Direction { FORWARD, BACK }

		public SplineRenderer splineRenderer;

		public TangentHandle linkTo;
		public Direction direction;

		private LineRenderer _lineRenderer;

		void Start() {
			_lineRenderer = GetComponent<LineRenderer>();
		}

		public Vector3 tangent {
			get {
				return (direction == Direction.BACK ? -transform.localPosition : transform.localPosition) * 3f;
			}
		}

		public void OnDrawGizmos() {
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.parent.position, transform.position);
		}

		public void Rotate(Quaternion rotation) {
			transform.localPosition = rotation * transform.localPosition;
			_lineRenderer.SetPosition(1, -transform.localPosition);
		}

		void UIActiveHandle(RaycastHit hit) {
			Plane plane = new Plane(Vector3.up, transform.position);
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			float dist;
			if(plane.Raycast(ray, out dist)) {
				Vector3 mousePoint = ray.GetPoint(dist);
				Vector3 localPoint = mousePoint - transform.parent.position;

				//transform.parent.rotation = Quaternion.LookRotation(localPoint);

				if(linkTo != null) {
					Quaternion rotation = Quaternion.FromToRotation(transform.localPosition, localPoint);
					linkTo.Rotate(rotation);
				}

				transform.localPosition = localPoint;
				_lineRenderer.SetPosition(1, -localPoint);
			}
		}
	}
}

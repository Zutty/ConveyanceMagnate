using UnityEngine;
using System.Collections;

public class CameraPanZoom : MonoBehaviour {

	public float zoomSpeed = 2f;
	public float minimumHeight = 0f;
	public float maximumHeight = 100f;

	private Plane _ground;
	private Camera _camera;
	private Vector3 _prevPan;

	public void Start() {
		_ground = new Plane(Vector3.up, Vector3.zero);
		_camera = GetComponent<Camera>();
		_prevPan = Vector3.zero;
	}

	public void Update () {
		Vector3 pos = transform.position;

		float zoom = Input.GetAxis("Mouse ScrollWheel");
		if(zoom != 0f) {
			float max = (transform.forward * ((pos.y - (Mathf.Sign(zoom) < 0 ? maximumHeight : minimumHeight)) / transform.forward.y)).magnitude;
			pos += Vector3.ClampMagnitude(zoom * zoomSpeed * transform.forward, max);
		}

		if (Input.GetMouseButton(1)) {
			Vector3 pan = pos - MousePositionOnGround();

			if (!Input.GetMouseButtonDown(1)) {
				pos += pan - _prevPan;
			}

			_prevPan = pan;
		}

		transform.position = pos;
	}

	private Vector3 MousePositionOnGround() {
		Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
		float distance = 0f;
		_ground.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}
}

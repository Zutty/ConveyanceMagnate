using UnityEngine;
using System.Collections;

public class CameraPanZoom : MonoBehaviour {

	public float panSpeed = 2f;
	public float zoomSpeed = 2f;

	void Update () {
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		if(zoom != 0f) {
			transform.position += Mathf.Sign(zoom) * zoomSpeed * transform.forward;
		}

		if(Input.GetMouseButton(1)) {
			transform.position += Input.GetAxis("Mouse X") * panSpeed * Vector3.forward;
			transform.position -= Input.GetAxis("Mouse Y") * panSpeed * Vector3.right;
		}
	}
}

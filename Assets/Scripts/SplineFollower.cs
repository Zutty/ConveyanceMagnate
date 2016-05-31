using UnityEngine;
using System.Collections;

public class SplineFollower : MonoBehaviour {

	public float speed = 0.5f;
	public EditableSpline spline;

	private float distance = 0f;

	void Update () {
		distance += speed * Time.deltaTime;

		spline.UpdateTransform (distance, transform);
	}
}

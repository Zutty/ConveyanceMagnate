using UnityEngine;
using System.Collections;
using Spline;

public class SplineFollower : MonoBehaviour {

	public float speed = 0.5f;
	public SplineIntegrator spline;
	public float trail = 0f;

	private float distance = 0f;

	void Update () {
		distance += speed * Time.deltaTime;

		if(trail == 0f) {
			spline.UpdateTransform(distance, transform);
		} else {
			spline.UpdateTransformTrailing(distance, trail, transform);
		}
	}
}

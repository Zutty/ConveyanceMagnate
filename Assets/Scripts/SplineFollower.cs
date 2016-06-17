using UnityEngine;
using System.Collections;
using Spline;

public class SplineFollower : MonoBehaviour {

	public float speed = 0.5f;
	public SplineIntegrator spline;
	public Transform frontFollower;
	public Transform rearFollower;
	public float baseLength;

	private float distance = 0f;

	void Update () {
		distance += speed * Time.deltaTime;

		SplinePoint front = spline.GetPoint(distance);
		SplinePoint rear = spline.GetPointTrailing(distance, front.position, baseLength);

		frontFollower.position = front.position;
		frontFollower.rotation = front.rotation;

		rearFollower.position = rear.position;
		rearFollower.rotation = rear.rotation;

		transform.position = (front.position + rear.position) / 2f;
		transform.rotation = Quaternion.LookRotation(front.position - rear.position);
	}
}

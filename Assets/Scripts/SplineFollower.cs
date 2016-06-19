using UnityEngine;
using System.Collections;
using Spline;

public class SplineFollower : MonoBehaviour {

	public float speed = 0.5f;
	public SplineIntegrator spline;
	public Transform frontFollower;
	public Transform rearFollower;
	public float baseLength;
	public float bufferLength = 2f;

	private float distance = 0f;
	private bool _move = false;

	public void Start() {
		distance = baseLength + bufferLength;
		Move();
	}

	public void Update () {
		if(Input.GetKey(KeyCode.M)) {
			_move = true;
			distance = baseLength + bufferLength;
		}
		if(_move) {
			Move();
		}
	}

	public void Move() {
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

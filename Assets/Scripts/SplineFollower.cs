using UnityEngine;
using System.Collections.Generic;
using Spline;

public class SplineFollower : MonoBehaviour {

	public float speed = 0.5f;
	public SplineIntegrator spline;
	public List<SplineFollowerUnit> units;
	public float coupleDistance = 0f;

	private float distance = 0f;
	private bool _move = false;
	private float _straightLength;

	public float straightLength {
		get { return _straightLength; }
	}

	public void Start() {
		CalculateLength();
		distance = _straightLength;
		_move = true;
		Move();
	}

	private void CalculateLength() {
		_straightLength = coupleDistance * (units.Count - 1f);
		for(int i = 0; i < units.Count; i++) {
			_straightLength += units[i].straightLength;
		}
	}

	public void Update () {
		if(_move) {
			Move();
		}
	}

	public void Move() {
		distance += speed * Time.deltaTime;

		Vector3 com = Vector3.zero;
		float unitPosition = distance;

		for(int i = 0; i < units.Count; i++) {
			unitPosition -= units[i].bufferLength;
				
			SplinePoint front = spline.GetPoint(unitPosition);
			SplinePoint rear = spline.GetPointTrailing(unitPosition, front.position, units[i].baseLength);

			units[i].frontFollower.position = front.position;
			units[i].frontFollower.rotation = front.rotation;

			units[i].rearFollower.position = rear.position;
			units[i].rearFollower.rotation = rear.rotation;

			units[i].transform.position = (front.position + rear.position) / 2f;
			units[i].transform.rotation = Quaternion.LookRotation(front.position - rear.position);

			com += units[i].transform.position;
			unitPosition = spline.ArcLength(rear.t) - (units[i].bufferLength + coupleDistance);
		}

		transform.position = com / (float)units.Count;
	}
}

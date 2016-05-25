using UnityEngine;
using System.Collections;

public class FollowTangent : MonoBehaviour {

	public Transform target;
	public Transform tangent;
	public float distance = 5f;

	void Update () {
		transform.position = target.position + (target.position - tangent.position).normalized * distance;
	}

	void OnDrawGizmos() {
		Update();
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, 0.3f);
	}
}

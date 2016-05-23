using UnityEngine;
using System.Collections;

public class EditableSpline : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.white;

		foreach(Transform child in transform) {
			Gizmos.DrawWireSphere(child.position, 0.3f);
		}
	}
}

using UnityEngine;
using System.Collections;

public class Handle : MonoBehaviour {

	public LayerMask layerMask;

	private Projector _projector;

	void Start () {
		_projector = GetComponent<Projector> ();
	}
	
	void Update () {
		
	}

	void OnMouseOver() {
		//_renderer.material.SetColor("_TintColor", Color.blue);
	}

	void OnMouseExit() {
		//_renderer.material.SetColor("_TintColor", Color.white);
	}

	void OnMouseDrag() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000, layerMask) && hit.collider.gameObject.CompareTag("terrain")) {
			transform.position = hit.point;
		}
	}
}

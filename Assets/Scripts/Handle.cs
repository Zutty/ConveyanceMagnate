using UnityEngine;
using System.Collections;

public class Handle : MonoBehaviour {

	public static readonly Color PURPLE = new Color(229, 29, 137);

	public LayerMask layerMask;

	private Projector _projector;

	void Start () {
		_projector = GetComponent<Projector> ();
	}
	
	void Update () {
		
	}

	void OnMouseOver() {
		//_projector.material.SetColor("_Color", Color.blue);
	}

	void OnMouseExit() {
		//_projector.material.SetColor("_Color", PURPLE);
	}

	void OnMouseDrag() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000, layerMask) && hit.collider.gameObject.CompareTag("terrain")) {
			transform.parent.position = hit.point;
		}
	}
}

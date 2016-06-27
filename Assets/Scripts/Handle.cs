﻿using UnityEngine;
using System.Collections;

public class Handle : MonoBehaviour {

	public static readonly Color PURPLE = new Color(229, 29, 137);

	public enum HandleMode { DRAG, MOVE }

	public HandleMode mode = HandleMode.DRAG;
	public Material selectMaterial;
	public LayerMask layerMask;

	private Projector _projector;
	private Material _baseMaterial;

	public void Start () {
		_projector = GetComponent<Projector> ();
		_baseMaterial = _projector.material;
	}
	
	public void Update () {
		if(UIStateManager.instance.IsInState(UIStateManager.UIState.NORMAL) && mode == HandleMode.MOVE) {
			Reposition();
		}
	}

	public void OnMouseDrag() {
		if(UIStateManager.instance.IsInState(UIStateManager.UIState.NORMAL) && mode == HandleMode.DRAG) {
			Reposition();
		}
	}

	private void Reposition() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000, layerMask) && hit.collider.gameObject.CompareTag("terrain")) {
			transform.parent.position = hit.point;
		}
	}

	public void OnMouseOver() {
		_projector.material = selectMaterial;
	}

	public void OnMouseExit() {
		_projector.material = _baseMaterial;
	}
}

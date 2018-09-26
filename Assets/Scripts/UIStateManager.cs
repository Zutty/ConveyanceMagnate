using UnityEngine;
using System.Collections;
using Spline;

public class UIStateManager : MonoBehaviour {

	public enum UIState { NORMAL, CREATE_TRACK, DRAG_TRACK, BUILD_TRAIN }

	private static UIStateManager _instance;

	public static UIStateManager instance {
		get { return _instance; }
	}

	public GameObject createTrackHandle;
	public GameObject trackPrefab;
	public SplineFollower trainPrefab;

	public LayerMask groundLayerMask;
	public LayerMask uiLayerMask;

	private UIState _state = UIState.NORMAL;
	//private Transform _draggable;
	private Projector _trackHandleProjector;
	private GameObject _activeHandle;

	void Start () {
		_instance = this;
		//_trackHandleProjector = createTrackHandle.GetComponentInChildren<Projector>();
		//_trackHandleProjector.enabled = false;
	}
	
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 mousePosition = Vector3.zero;

		RaycastHit groundHit;
		if (Physics.Raycast(ray, out groundHit, 1000, groundLayerMask) && groundHit.collider.gameObject.CompareTag("terrain")) {
			mousePosition = groundHit.point;
		}

		RaycastHit uiHit;
		bool isUiHit = Physics.Raycast(ray, out uiHit, 1000, uiLayerMask) && uiHit.collider.gameObject.CompareTag("SplineHandle");

		if(Input.GetMouseButtonUp(0)) {
			_activeHandle = null;
		}

		if(isUiHit && Input.GetMouseButtonDown(0)) {
			_activeHandle = uiHit.collider.gameObject;
		}

		if(_activeHandle != null && Input.GetMouseButton(0)) {
			_activeHandle.SendMessageUpwards("UIActiveHandle", uiHit, SendMessageOptions.DontRequireReceiver);
		}
	}

	public UIState state {
		get { return _state; }
	}

	public void ClearState() {
		_state = UIState.NORMAL;
		//_draggable = null;
	}

	public bool IsInState(UIState state) {
		return _state == state;
	}

	public void TriggerCreateTrack() {
		_state = UIState.CREATE_TRACK;
		//_trackHandleProjector.enabled = true;
	}

	public void TriggerBuildTrain() {
		_state = UIState.BUILD_TRAIN;
	}
}

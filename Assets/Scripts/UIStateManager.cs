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
	private Transform _draggable;
	private Projector _trackHandleProjector;

	void Start () {
		_instance = this;
		_trackHandleProjector = createTrackHandle.GetComponentInChildren<Projector>();
		_trackHandleProjector.enabled = false;
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

		// TODO Split this bullshit into separate classes you goit
		if(_state == UIState.CREATE_TRACK) {
			_trackHandleProjector.enabled = !isUiHit;

			createTrackHandle.transform.position = mousePosition;
				
			if(Input.GetMouseButtonDown(0)) {
				_state = UIState.DRAG_TRACK;
				_trackHandleProjector.enabled = false;

				GameObject newTrack = (GameObject)Instantiate(trackPrefab);
				CompositeSpline spline = newTrack.GetComponent<CompositeSpline>();

				if(isUiHit) {
					// Reuse existing point
					spline.points[1] = uiHit.collider.gameObject.transform;
				} else {
					// Create new point
					spline.points[1].position = mousePosition;
				}

				_draggable = spline.points[2];
				_draggable.position = mousePosition;
			}
		} else if(_state == UIState.DRAG_TRACK) {
			_draggable.position = mousePosition;
			if(Input.GetMouseButtonUp(0)) {
				ClearState();
			}
		} else if(_state == UIState.BUILD_TRAIN) {
			if(Input.GetMouseButtonDown(0)) {
				ClearState();
				RaycastHit splineHit;
				if (Physics.Raycast(ray, out splineHit, 1000) && splineHit.collider.gameObject.CompareTag("spline")) {
					Transform segment = splineHit.collider.transform.parent;

					SplineFollower train = (SplineFollower)Instantiate(trainPrefab);
					train.spline = segment.GetComponentInParent<SplineIntegrator>();
				}
			}
		}
	}

	public UIState state {
		get { return _state; }
	}

	public void ClearState() {
		_state = UIState.NORMAL;
		_draggable = null;
	}

	public bool IsInState(UIState state) {
		return _state == state;
	}

	public void TriggerCreateTrack() {
		_state = UIState.CREATE_TRACK;
		_trackHandleProjector.enabled = true;
	}

	public void TriggerBuildTrain() {
		_state = UIState.BUILD_TRAIN;
	}
}

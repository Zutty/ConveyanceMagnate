using UnityEngine;
using System.Collections;

public class UIStateManager : MonoBehaviour {

	public enum UIState { NORMAL, CREATE_TRACK }

	public GameObject trackPrefab;

	private static UIStateManager _instance;

	private UIState _state = UIState.NORMAL;

	void Start () {
		_instance = this;
	}
	
	void Update () {
	
	}

	public static UIStateManager instance {
		get { return _instance; }
	}

	public UIState state {
		get { return _state; }
	}

	public void ClearState() {
		_state = UIState.NORMAL;
	}

	public void TriggerCreateTrack() {
		Instantiate(trackPrefab);
	}
}

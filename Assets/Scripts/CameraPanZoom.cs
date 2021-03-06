﻿using UnityEngine;

public class CameraPanZoom : MonoBehaviour {
    public float zoomSpeed = 2f;
    public float minimumHeight;
    public float maximumHeight = 100f;

    private Plane _ground;
    private Camera _camera;
    private Vector3 _prevPan;

    public void Start() {
        _ground = new Plane(Vector3.up, Vector3.zero);
        _camera = GetComponent<Camera>();
        _prevPan = Vector3.zero;
    }

    public void Update() {
        var pos = transform.position;

        var zoom = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(zoom, 0f)) {
            var max = (transform.forward *
                         ((pos.y - (Mathf.Sign(zoom) < 0 ? maximumHeight : minimumHeight)) / transform.forward.y))
                .magnitude;
            pos += Vector3.ClampMagnitude(zoom * zoomSpeed * transform.forward, max);
        }

        if (Input.GetMouseButton(1)) {
            var pan = pos - MousePositionOnGround();

            if (!Input.GetMouseButtonDown(1)) {
                pos += pan - _prevPan;
            }

            _prevPan = pan;
        }

        transform.position = pos;
    }

    private Vector3 MousePositionOnGround() {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        float distance;
        _ground.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
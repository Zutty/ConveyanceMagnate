using System;
using UnityEngine;

namespace Spline {
    [RequireComponent(typeof(LineRenderer))]
    public class TangentHandle : MonoBehaviour {
        public const float TANGENT_MULTIPLIER = 3f;

        public enum Direction {
            FORWARD,
            BACK
        }

        public Direction direction;

        private LineRenderer _lineRenderer;
        private ControlPoint _controlPoint;

        void Start() {
            _lineRenderer = GetComponent<LineRenderer>();
            _controlPoint = GetComponentInParent<ControlPoint>();
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.parent.position, transform.position);
        }

        public void Rotate(Quaternion rotation) {
            transform.localPosition = rotation * transform.localPosition;
            _lineRenderer.SetPosition(1, -transform.localPosition);
        }

        private bool isForward {
            get { return direction == Direction.FORWARD; }
        }

        private Vector3 directional(Vector3 v) {
            return isForward ? v : -v;
        }

        private static Vector3 planeRayIntersection(Vector3 origin, Ray ray) {
            var plane = new Plane(Vector3.up, origin);

            float dist;
            if (!plane.Raycast(ray, out dist)) {
                throw new ApplicationException("Ray and plane are parallel");
            }

            return ray.GetPoint(dist);
        }

        void UIActiveHandle(RaycastHit hit) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mousePoint = planeRayIntersection(_controlPoint.position, ray);
            var localPoint = directional(mousePoint - _controlPoint.position);

            _controlPoint.transform.rotation = Quaternion.LookRotation(localPoint);

            var len = localPoint.magnitude;

            if (isForward) {
                _controlPoint.forwardLength = len * TANGENT_MULTIPLIER;
            } else {
                _controlPoint.backLength = len * TANGENT_MULTIPLIER;
            }

            var handlePosition = directional(Vector3.forward * len);

            transform.localPosition = handlePosition;
            _lineRenderer.SetPosition(1, -handlePosition);
        }
    }
}
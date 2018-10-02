using UnityEngine;

namespace Spline {
    [RequireComponent(typeof(ControlEdge))]
    public class CompoundCollider : MonoBehaviour {
        public int segments = 5;
        public float width = 2f;

        private ControlEdge _edge;

        private void Start() {
            _edge = GetComponent<ControlEdge>();
        }

        public void Update() {
            var children = GetComponentsInChildren<BoxCollider>();

            if (children.Length < segments) {
                for (var i = 0; i < segments - children.Length; i++) {
                    var child = new GameObject();
                    child.AddComponent<BoxCollider>();
                    child.transform.parent = transform;
                }
            } else if (children.Length > segments) {
                for (var i = segments; i < children.Length; i++) {
                    Destroy(children[i].gameObject);
                }
            }

            children = GetComponentsInChildren<BoxCollider>();
            var curve = _edge.curve;
            var interval = curve.arcLength / segments;

            for (var i = 0; i < segments; i++) {
                var s = i / (float) segments * curve.arcLength + interval / 2;
                var t = Splines.GetCurveParameter(curve, s);

                children[i].transform.position = curve.GetPosition(t);
                children[i].transform.rotation = Quaternion.LookRotation(curve.GetTangent(t));
                children[i].size = new Vector3(width, 1f, interval);
            }
        }
    }
}
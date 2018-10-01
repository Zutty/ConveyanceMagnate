using UnityEngine;

namespace Spline {
    public class PositionHandle : MonoBehaviour {
        public LayerMask terrainLayerMask;

        public Vector3 position {
            get { return transform.position; }
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }

        void UIActiveHandle(RaycastHit uiHit) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, terrainLayerMask) &&
                hit.collider.gameObject.CompareTag("terrain")) {
                transform.parent.position = hit.point;
            }
        }
    }
}
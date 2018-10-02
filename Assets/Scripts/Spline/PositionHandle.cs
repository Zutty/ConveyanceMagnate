using UnityEngine;

namespace Spline {
    public class PositionHandle : MonoBehaviour {
        public void OnDrawGizmos() {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }

        void UIActiveHandle(RaycastHit uiHit) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            transform.parent.position = TangentHandle.PlaneRayIntersection(transform.parent.position, ray);
        }
    }
}
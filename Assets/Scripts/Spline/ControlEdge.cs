using System;
using UnityEngine;

namespace Spline {
    public class ControlEdge : MonoBehaviour {
        public ControlPoint a;
        public ControlPoint b;

        public ControlEdge aNeighbor;
        public ControlEdge bNeighbor;

        public bool invert;
        
        public ControlEdge NextAwayFrom(ControlPoint p) {
            if (p != a && p != b) {
                throw new Exception("This doesnt make sense");
            }

            return p == a ? bNeighbor : aNeighbor;
        }

        public CubicCurve curve {
            get {
                HermiteForm hermiteForm;
                hermiteForm.p0 = a.position;
                hermiteForm.m0 = a.forwardTangent;
                hermiteForm.p1 = b.position;
                hermiteForm.m1 = invert ? -b.backTangent : b.backTangent;
                return Splines.HermiteSpline(hermiteForm);
            }
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.blue;

            var prev = a.position;

            for (var t = 0.05f; t < 1f; t += 0.05f) {
                var p = curve.basis.Solve(t);
                Gizmos.DrawLine(prev, p);
                prev = p;
            }

            Gizmos.DrawLine(prev, b.position);
            
            Gizmos.color = Color.green;

            var reverse = Splines.HermiteSpline(new HermiteForm {
                p0 = b.position,
                m0 = -b.forwardTangent,
                p1 = a.position,
                m1 = invert ? a.backTangent : -a.backTangent
            });

            for (var t = 0f; t < 1f; t += 0.05f) {
                Gizmos.DrawLine(reverse.GetPosition(t), reverse.GetPosition(t + 0.05f));
            }
        }
    }
}
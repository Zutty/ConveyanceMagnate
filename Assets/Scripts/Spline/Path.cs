using System;

namespace Spline {
    [Serializable]
    public class Path {
        public ControlEdge edge;
        public ControlPoint point;

        /*
        public Path(ControlEdge edge, ControlPoint point) {
            if (point != edge.a && point != edge.b) {
                throw new Exception("This doesnt make sense");
            }

            _edge = edge;
            _point = point;
        }
*/
        private ControlPoint _opposite {
            get { return point == edge.a ? edge.b : edge.a; }
        }

        public Path WalkForward() {
            var oppositeNeighbour = point == edge.a ? edge.bNeighbor : edge.aNeighbor;
            return oppositeNeighbour != null
                ? new Path {
                    edge = oppositeNeighbour,
                    point = _opposite
                }
                : this;
        }

        public Path WalkBack() {
            var nearNeighbour = point == edge.a ? edge.aNeighbor : edge.bNeighbor;
            return nearNeighbour != null
                ? new Path {
                    edge = nearNeighbour, 
                    point = point == nearNeighbour.a ? nearNeighbour.b : nearNeighbour.a
                }
                : this;
        }

        public Path Walk(int offset) {
            return WalkRecursive(this, offset);
        }

        private static Path WalkRecursive(Path p, int n) {
            while (n != 0) {
                if (n > 0) {
                    p = p.WalkForward();
                    n--;
                    continue;
                }

                p = p.WalkBack();
                n++;
            }

            return p;
        }

        public CubicCurve GetCurve() {
            return Splines.HermiteSpline(new HermiteForm {
                p0 = point.position,
                m0 = point.forwardTangent,
                p1 = _opposite.position,
                m1 = _opposite.backTangent
            });
        }
    }
}
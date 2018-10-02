using UnityEngine;

namespace Spline {
    [RequireComponent(typeof(ControlEdge))]
    public class Extrude : MonoBehaviour {
        public ExtrudableShape shape;

        private ControlEdge _edge;
        private Mesh mesh;

        private int splineLen;
        private int[] triangles;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;

        public void Start() {
            _edge = GetComponent<ControlEdge>();
            mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();

            Resize(EstimateSplineLen());
        }

        private int EstimateSplineLen() {
            return Mathf.CeilToInt(_edge.curve.arcLength / 2f);
        }

        public void Update() {
            var len = EstimateSplineLen();

            if (len != splineLen) {
                Resize(len);
            } else {
                Recalculate();
            }
        }

        public void Resize(int newLength) {
            if (newLength <= 1) {
                splineLen = 0;
                triangles = new int[0];
                vertices = new Vector3[0];
                normals = new Vector3[0];
                uv = new Vector2[0];
                return;
            }

            splineLen = newLength;
            var vertexCount = shape.vertices.Length * newLength;
            var indexCount = shape.lines.Length * (newLength - 1) * 3;

            triangles = new int[indexCount];
            vertices = new Vector3[vertexCount];
            normals = new Vector3[vertexCount];
            uv = new Vector2[vertexCount];

            Recalculate();
        }

        public void Recalculate() {
            if (splineLen == 0) {
                return;
            }

            var shapeVertices = shape.vertices.Length;

            var idx = 0;
            foreach (var p in Splines.Sample(_edge.curve, splineLen)) {
                for (var j = 0; j < shapeVertices; j++, idx++) {
                    vertices[idx] = transform.InverseTransformPoint(p.position + p.orientation * shape.vertices[j]);
                    normals[idx] = p.orientation * shape.normals[j];
                    uv[idx] = new Vector2(shape.u[j], p.len / 2f);
                }
            }

            var ti = 0;
            for (var i = 0; i < splineLen - 1; i++) {
                var offset = i * shapeVertices;
                for (var l = 0; l < shape.lines.Length; l += 2) {
                    var pa = offset + shape.lines[l] + shapeVertices;
                    var pb = offset + shape.lines[l];
                    var pc = offset + shape.lines[l + 1];
                    var pd = offset + shape.lines[l + 1] + shapeVertices;
                    triangles[ti++] = pa;
                    triangles[ti++] = pb;
                    triangles[ti++] = pc;
                    triangles[ti++] = pc;
                    triangles[ti++] = pd;
                    triangles[ti++] = pa;
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
        }
    }
}
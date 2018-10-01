using UnityEngine;

namespace Spline {
    public class Extrude : MonoBehaviour {
        public ExtrudableShape shape;
        //public GameObject colliderSegmentPrefab;
        //public float collisionWidth = 1f;

        private ControlEdge spline;
        private Mesh mesh;

        private int splineLen;
        private int[] triangles;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;

        //private List<BoxCollider> _colliderSegments = new List<BoxCollider>();

        void Start() {
            spline = GetComponent<ControlEdge>();
            mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();

            Resize(EstimateSplineLen());
        }

        private int EstimateSplineLen() {
            return Mathf.CeilToInt(spline.GetSpline().arcLength / 2f);
        }

        void Update() {
            var len = EstimateSplineLen();

            if (len != splineLen) {
                Resize(len);
            } else {
                Recalculate();
            }
        }

        void Resize(int splineLen) {
            if (splineLen <= 1) {
                this.splineLen = 0;
                triangles = new int[0];
                vertices = new Vector3[0];
                normals = new Vector3[0];
                uv = new Vector2[0];
                return;
            }

            this.splineLen = splineLen;
            var vertexCount = shape.vertices.Length * splineLen;
            var indexCount = shape.lines.Length * (splineLen - 1) * 3;

            triangles = new int[indexCount];
            vertices = new Vector3[vertexCount];
            normals = new Vector3[vertexCount];
            uv = new Vector2[vertexCount];
/*
			int diff = splineLen - 1 - _colliderSegments.Count;
			if(_colliderSegments.Count < splineLen - 1) {
				for(int i = 0; i < diff; i++) {
					GameObject colliderSegment = (GameObject)Instantiate(colliderSegmentPrefab);
					colliderSegment.transform.parent = transform;
					_colliderSegments.Add(colliderSegment.GetComponent<BoxCollider>());
				}
			} else if(_colliderSegments.Count > splineLen - 1) {
				for(int i = splineLen - 1; i < _colliderSegments.Count; i++) {
					Destroy(_colliderSegments[i].gameObject);
				}
				_colliderSegments.RemoveRange(splineLen - 1, _colliderSegments.Count - splineLen + 1);
			}
			if(splineLen - 1 != _colliderSegments.Count) {
				Debug.Log("splineLen - 1 = " + (splineLen - 1) + ", _colliderSegments.Count = " + _colliderSegments.Count);
			}
*/
            Recalculate();
        }

        void Recalculate() {
            if (splineLen == 0) {
                return;
            }

            var shapeVertices = shape.vertices.Length;

            int idx = 0, ci = -1;
            var prev = Vector3.zero;
            foreach (Splines.Point p in Splines.Sample(spline.GetSpline(), splineLen)) {
                for (int j = 0; j < shapeVertices; j++, idx++) {
                    vertices[idx] = transform.InverseTransformPoint(p.position + p.orientation * shape.vertices[j]);
                    normals[idx] = p.orientation * shape.normals[j];
                    uv[idx] = new Vector2(shape.u[j], p.len / 2f);
                }

                if (ci >= 0) {
                    //_colliderSegments[ci].transform.position = (p.position + prev) / 2;
                    //_colliderSegments[ci].transform.rotation = Quaternion.LookRotation(p.position - prev);
                    //_colliderSegments[ci].size = new Vector3(collisionWidth, 1f, Vector3.Distance(p.position, prev));
                }

                ci++;
                prev = p.position;
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
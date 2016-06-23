using UnityEngine;
using System.Collections.Generic;

namespace Spline {
	public class Extrude : MonoBehaviour {

		[System.Serializable]
		public struct Shape {
			public Vector2[] vertices;
			public Vector2[] normals;
			public float[] u;
			public int[] lines;
		}

		public int splineIndex;
		public Shape shape;
		public GameObject colliderSegmentPrefab;
		public float collisionWidth = 1f;

		private Mesh mesh;

		private int splineLen;
		private int[] triangles;
		private Vector3[] vertices;
		private Vector3[] normals;
		private Vector2[] uv;

		private List<BoxCollider> _colliderSegments = new List<BoxCollider>();
		private CompositeSpline _compositeSpline;

		void Start() {
			_compositeSpline = GetComponentInParent<CompositeSpline>();
			mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();

			Resize(EstimateSplineLen());
		}

		private CatmullRomSpline spline {
			get { return _compositeSpline[splineIndex]; }
		}

		private int EstimateSplineLen() {
			return Mathf.CeilToInt(spline.ArcLength(1f) / 2f);
		}

		void Update() {
			int len = EstimateSplineLen();

			if(len != splineLen) {
				Resize(len);
			} else {
				Recalculate();
			}
		}

		void Resize(int splineLen) {
			if(splineLen <= 1) {
				this.splineLen = 0;
				triangles = new int[0];
				vertices = new Vector3[0];
				normals = new Vector3[0];
				uv = new Vector2[0];
				return;
			}

			this.splineLen = splineLen;
			int vertexCount = shape.vertices.Length * splineLen;
			int indexCount = shape.lines.Length * (splineLen - 1) * 3;

			triangles = new int[ indexCount ];
			vertices = new Vector3[ vertexCount ];
			normals = new Vector3[ vertexCount ];
			uv = new Vector2[ vertexCount ];

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

			Recalculate();
		}

		void Recalculate() {
			if(splineLen == 0) {
				return;
			}

			int shapeVertices = shape.vertices.Length;

			int idx = 0, ci = -1;
			Vector3 prev = Vector3.zero;
			foreach(CatmullRomSpline.Point p in spline.Sample(splineLen)) {
				for(int j = 0; j < shapeVertices; j++, idx++) {
					vertices[idx] = transform.InverseTransformPoint(p.LocalToWorld(shape.vertices[j]));
					normals[idx] = p.LocalToWorldDirection(shape.normals[j]);
					uv[idx] = new Vector2(shape.u[j], p.len / 2f);
				}

				if(ci >= 0) {
					_colliderSegments[ci].transform.position = (p.position + prev) / 2;
					_colliderSegments[ci].transform.rotation = Quaternion.LookRotation(p.position - prev);
					_colliderSegments[ci].size = new Vector3(collisionWidth, 1f, Vector3.Distance(p.position, prev));
				}
				ci++;
				prev = p.position;
			}

			int ti = 0;
			for(int i = 0; i < splineLen - 1; i++) {
				int offset = i * shapeVertices;
				for(int l = 0; l < shape.lines.Length; l += 2) {
					int pa = offset + shape.lines[l] + shapeVertices;
					int pb = offset + shape.lines[l];
					int pc = offset + shape.lines[l + 1];
					int pd = offset + shape.lines[l + 1] + shapeVertices;
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

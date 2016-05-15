using UnityEngine;
using System.Collections;

public class Extrude : MonoBehaviour {

	[System.Serializable]
	public struct Shape {
		public Vector2[] vertices;
		public Vector2[] normals;
		public float[] u;
		public int[] lines;
	}

	public Shape shape;
	public Transform a;
	public Transform b;
	public Transform c;
	public Transform d;

	private CubicBezierSpline spline;
	private Mesh mesh;

	private CubicBezierSpline.Point[] splinePoints;
	private int[] triangles;
	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uv;

	void Start() {
		mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh ();
		spline = new CubicBezierSpline (a.position, b.position, c.position, d.position);
		Resize(16);
	}

	void Update() {
		Recalculate();
	}

	void Resize(int splineLen) {
		splinePoints = new CubicBezierSpline.Point[splineLen];

		Debug.Log(splinePoints);

		int vertexCount = shape.vertices.Length * splineLen;
		int indexCount = shape.lines.Length * (splineLen - 1) * 3;

		triangles = new int[ indexCount ];
		vertices = new Vector3[ vertexCount ];
		normals = new Vector3[ vertexCount ];
		uv = new Vector2[ vertexCount ];

		Recalculate();
	}

	void Recalculate() {
		spline.a = a.position;
		spline.b = b.position;
		spline.c = c.position;
		spline.d = d.position;
		spline.Sample(splinePoints);

		int splineLen = splinePoints.Length;
		int shapeVertices = shape.vertices.Length;

		for (int i = 0; i < splineLen; i++) {
			int offset = i * shapeVertices;
			for (int j = 0; j < shapeVertices; j++) {
				int id = offset + j;
				vertices [id] = splinePoints [i].LocalToWorld(shape.vertices [j]);
				normals [id] = splinePoints [i].LocalToWorldDirection(shape.normals [j]);
				uv [id] = new Vector2 (shape.u [j], i / (float)splineLen);
			}
		}

		int ti = 0;
		for (int i = 0; i < splineLen - 1; i++) {
			int offset = i * shapeVertices;
			for (int l = 0; l < shape.lines.Length; l += 2) {
				int pa = offset + shape.lines [l] + shapeVertices;
				int pb = offset + shape.lines [l];
				int pc = offset + shape.lines [l + 1];
				int pd = offset + shape.lines [l + 1] + shapeVertices;
				triangles[ti] = pa; 	ti++;
				triangles[ti] = pb; 	ti++;
				triangles[ti] = pc; 	ti++;
				triangles[ti] = pc; 	ti++;
				triangles[ti] = pd; 	ti++;
				triangles[ti] = pa; 	ti++;
			}
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
	}
}

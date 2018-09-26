using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshArray : MonoBehaviour {
	
	public Mesh mesh;
	public int count;
	public Vector3 offset; 
	
	private Mesh arrayMesh;

	void Start () {
		arrayMesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();
		Recalculate();
	}

	public void Recalculate() {
		int arrayVertexCount = mesh.vertexCount * count;
		int arrayTriangleCount = mesh.triangles.Length * count;
		
		var vertices = new Vector3[arrayVertexCount];
		var normals = new Vector3[arrayVertexCount];
		//var uv = new Vector2[arrayVertexCount];
		var triangles = new int[arrayTriangleCount];

		for (var n = 0; n < count; n++) {
			var vertexOffset = mesh.vertexCount * n;
			var triangleOffset = mesh.triangles.Length * n;
			
			for (var idx = 0; idx < mesh.vertexCount; idx++) {
				vertices[idx + vertexOffset] = mesh.vertices[idx] + (offset * n);
				normals[idx + vertexOffset] = mesh.normals[idx];
				//uv[idx + vertexOffset] = mesh.uv[idx];
			}
	
			for (var idx = 0; idx < mesh.triangles.Length; idx++) {
				triangles[idx + triangleOffset] = mesh.triangles[idx] + vertexOffset;
			}
		}

		arrayMesh.Clear();
		arrayMesh.vertices = vertices;
		arrayMesh.normals = normals;
		//arrayMesh.uv = uv;
		arrayMesh.triangles = triangles;
	}
}

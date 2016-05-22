using UnityEngine;
using System.Collections;

public class SplineView : MonoBehaviour {

	public Transform[] controlPoints;

	private CatmullRomSpline crSpline = new CatmullRomSpline ();

	// Use this for initialization
	void Start () {
		//crSpline = new CatmullRomSpline ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos() {

		for(int i = 0; i < controlPoints.Length; i++) {
			Gizmos.DrawWireSphere(controlPoints [i].position, 0.3f);
		}

		if(controlPoints.Length == 4) {
			float t = 0;
			Vector3 p;// = crSpline.GetCentripetalCR(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position, t);
			Vector3 q, tangent;
			for(int i = 0; i <= 10; i++) {
				//q = crSpline.GetCentripetalCR(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position, t);
				//tangent = crSpline.GetCentripetalCRTangent(controlPoints [0].position, controlPoints [1].position, controlPoints [2].position, controlPoints [3].position, t).normalized;

				Gizmos.color = Color.yellow;
				//Gizmos.DrawRay(new Ray (q, tangent));

				Gizmos.color = Color.white;
				//Gizmos.DrawLine(p, q);

				t += 0.1f;
				//p = q;
			}
		}
	}


}

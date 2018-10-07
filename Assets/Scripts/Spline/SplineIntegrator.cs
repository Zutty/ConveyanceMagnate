using UnityEngine;

namespace Spline {
    public class SplineIntegrator : MonoBehaviour {
        //private CompositeSpline _spline;

        public void Start() {
            _spline = GetComponent<CompositeSpline>();
        }


}
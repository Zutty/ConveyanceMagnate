using UnityEngine;
using System.Collections.Generic;

namespace Spline {
    public class CompositeSpline : MonoBehaviour {
        public List<Transform> points;

        private readonly List<CubicCurve> _curves = new List<CubicCurve>();


    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline {
    public class ControlPoint : MonoBehaviour {
        public float forwardLength;
        public float backLength;

        public Vector3 position {
            get { return transform.position; }
        }

        public Vector3 forwardTangent {
            get { return transform.forward * forwardLength; }
        }

        public Vector3 backTangent {
            get { return transform.forward * backLength; }
        }
    }
}
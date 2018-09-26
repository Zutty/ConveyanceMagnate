using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spline {
    public class ControlPoint : MonoBehaviour {

        public TangentHandle forwardHandle;
        public TangentHandle backHandle;

        public Vector3 position {
            get { return transform.position; }
        }

        public Vector3 forwardTangent {
            get { return forwardHandle.tangent; }
        }
        public Vector3 backTangent {
            get { return backHandle.tangent; }
        }

    }
}
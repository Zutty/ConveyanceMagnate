using UnityEngine;

namespace Spline {
    public class SplineFollowerUnit : MonoBehaviour {
        public Transform frontFollower;
        public Transform rearFollower;
        public float baseLength;
        public float bufferLength = 2f;

        public float straightLength {
            get { return baseLength + (bufferLength * 2f); }
        }
    }
}
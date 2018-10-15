using UnityEngine;
using System.Collections.Generic;

namespace Spline {
    public class SplineFollower : MonoBehaviour {
        
        public ContinuousCurve curve;
        public float distance;
        
        public float speed = 0.5f;
        public List<SplineFollowerUnit> units;
        public float coupleDistance;

        public float straightLength { get; private set; }

        public void Start() {
            curve = GetComponent<ContinuousCurve>();
            CalculateLength();
            Reposition();
        }

        private void CalculateLength() {
            straightLength = coupleDistance * (units.Count - 1f);
            foreach (var unit in units) {
                straightLength += unit.straightLength;
             }
        }

        public void Update() {
            distance = curve.Advance(distance + speed * Time.deltaTime);
            Reposition();
        }

        public void Reposition() {
            var com = Vector3.zero;
            var unitPosition = distance;

            foreach (var unit in units) {
                unitPosition -= unit.bufferLength;
                
                var front = curve.GetPoint(unitPosition);
                var rear = curve.GetPointTrailing(unitPosition, front.position, unit.baseLength);

                unit.frontFollower.position = front.position;
                unit.frontFollower.rotation = front.rotation;

                unit.rearFollower.position = rear.position;
                unit.rearFollower.rotation = rear.rotation;

                unit.transform.position = (front.position + rear.position) / 2f;
                unit.transform.rotation = Quaternion.LookRotation(front.position - rear.position);

                com += unit.transform.position;
                unitPosition = rear.s - (unit.bufferLength + coupleDistance);
            }

            transform.position = com / units.Count;
        }
    }
}
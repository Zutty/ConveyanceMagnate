using UnityEngine;
using System.Collections.Generic;

namespace Spline {
    public class SplineFollower : MonoBehaviour {
        
        public ContinuousCurve curve;
        public Position position;
        public float speed = 0.5f;
        public List<SplineFollowerUnit> units;
        public float coupleDistance;

        public float straightLength { get; private set; }

        public void Start() {
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
            position = curve.Move(position, speed * Time.deltaTime);
            Reposition();
        }

        public void Reposition() {
            var com = Vector3.zero;
            var unitPosition = position;

            foreach (var unit in units) {
                unitPosition.s -= unit.bufferLength;

                var front = spline.GetPoint(unitPosition);
                var rear = spline.GetPointTrailing(unitPosition, front.position, unit.baseLength);

                unit.frontFollower.position = front.position;
                unit.frontFollower.rotation = front.rotation;

                unit.rearFollower.position = rear.position;
                unit.rearFollower.rotation = rear.rotation;

                unit.transform.position = (front.position + rear.position) / 2f;
                unit.transform.rotation = Quaternion.LookRotation(front.position - rear.position);

                com += unit.transform.position;
                unitPosition.s = rear.s - (unit.bufferLength + coupleDistance);
            }

            transform.position = com / units.Count;
        }
    }
}
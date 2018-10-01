using UnityEngine;
using System.Collections.Generic;

namespace Spline {
    public class SplineFollower : MonoBehaviour {
        public float speed = 0.5f;
        public SplineIntegrator spline;
        public List<SplineFollowerUnit> units;
        public float coupleDistance;

        private float distance;
        private bool _move;
        private float _straightLength;

        public float straightLength {
            get { return _straightLength; }
        }

        public void Start() {
            CalculateLength();
            distance = _straightLength;
            _move = true;
            Move();
        }

        private void CalculateLength() {
            _straightLength = coupleDistance * (units.Count - 1f);
            for (int i = 0; i < units.Count; i++) {
                _straightLength += units[i].straightLength;
            }
        }

        public void Update() {
            if (_move) {
                Move();
            }
        }

        public void Move() {
            distance += speed * Time.deltaTime;

            var com = Vector3.zero;
            var unitPosition = distance;

            foreach (var unit in units) {
                unitPosition -= unit.bufferLength;

                SplinePoint front = spline.GetPoint(unitPosition);
                SplinePoint rear = spline.GetPointTrailing(unitPosition, front.position, unit.baseLength);

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
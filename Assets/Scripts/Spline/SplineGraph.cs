using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spline {
    public class SplineGraph : MonoBehaviour {
        private struct IndexKey : IEquatable<IndexKey> {
            private readonly ControlPoint a;
            private readonly ControlPoint b;

            public IndexKey(ControlPoint a, ControlPoint b) {
                this.a = a;
                this.b = b;
            }

            public bool Equals(IndexKey other) {
                return a.Equals(other.a) && b.Equals(other.b);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                return obj is IndexKey && Equals((IndexKey) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return (Math.Min(a.GetHashCode(), b.GetHashCode()) * 397) ^ Math.Max(a.GetHashCode(), b.GetHashCode());
                }
            }
        }
        
        private readonly Dictionary<IndexKey, ControlEdge> _index = new Dictionary<IndexKey, ControlEdge>();

        public void Start() {
            RebuildIndex();
        }

        private void RebuildIndex() {
            _index.Clear();

            foreach (var edge in GetComponentsInChildren<ControlEdge>()) {
                _index[new IndexKey(edge.a, edge.b)] = edge;
            }
        }

        public CubicCurve GetCurve(Position position) {
            var key = new IndexKey(position.a, position.b);
            if (!_index.ContainsKey(key)) {
                throw new UnityException();
            }
            
            return _index[key].curve;
        }

        public ControlPoint GetNeighbor(ControlPoint p) {
            
        }
    }
}
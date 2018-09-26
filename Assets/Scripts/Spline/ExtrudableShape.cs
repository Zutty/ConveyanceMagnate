using UnityEngine;

namespace Spline {
    [CreateAssetMenu]
    public class ExtrudableShape : ScriptableObject {
        public Vector2[] vertices;
        public Vector2[] normals;
        public float[] u;
        public int[] lines;
    }
}
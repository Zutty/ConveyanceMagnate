using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spline;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineGraph))]
public class GraphCheat : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Diddle Graph")) {
            DiddleGraph((SplineGraph)target);
        }
    }

    private void DiddleGraph(SplineGraph graph) {
        var index = new Dictionary<ControlPoint, HashSet<ControlEdge>>();
            
        foreach (var edge in graph.GetComponentsInChildren<ControlEdge>()) {
            if (!index.ContainsKey(edge.a)) {
                index[edge.a] = new HashSet<ControlEdge>();
            }

            index[edge.a].Add(edge);
            
            if (!index.ContainsKey(edge.b)) {
                index[edge.b] = new HashSet<ControlEdge>();
            }

            index[edge.b].Add(edge);
        }
        
        foreach (var edge in graph.GetComponentsInChildren<ControlEdge>()) {
            edge.aNeighbor = index[edge.a].First(e => e != edge);
            edge.bNeighbor = index[edge.b].First(e => e != edge);
            EditorUtility.SetDirty(edge);
        }
    }
}

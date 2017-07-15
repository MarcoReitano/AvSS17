using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Graph_Behaviour))]
public class Graph_Behaviour_Editor : Editor
{
    [SerializeField]
    private Graph_Behaviour graphBehaviour;

    [SerializeField]
    private bool showDefaultInspector;
    private static bool edgesFoldout = true;
    private static bool nodesFoldout = true;


    public void Awake()
    {
        graphBehaviour = target as Graph_Behaviour;
    }



    public void OnSceneGUI()
    {
        Event currentEvent = Event.current;
        Vector3 worldPoint = InputHelpers.GetXZPlaneCollisionInEditorFixedHeight(0f, currentEvent);
        Debug.Log(worldPoint);

        if (InputHelpers.ClickedLeft())
        {
            //Vector3 worldPoint = InputHelpers.GetXZPlaneCollisionInEditorFixedHeight(0f, currentEvent);
            //Debug.Log(worldPoint);

            if (Event.current.control)
                worldPoint = worldPoint.SnapToGrid();

            Node_Behaviour nodeBehaviour = graphBehaviour.CreateNodeBehaviour(worldPoint);

            nodeBehaviour.gameObject.SelectInEditor();
        }

        //if (GUI.changed)
        //    EditorUtility.SetDirty(graphBehaviour);
    }




    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Create Node"))
        {
            graphBehaviour.CreateNodeBehaviour(Vector3.zero);
        }


        //if (nodesFoldout = EditorGUILayout.Foldout(nodesFoldout, "Nodes (" + graph.Nodes.Count + ")"))
        //{
        //    CustomGUIUtils.BeginGroup();
        //    int count = 1;
        //    foreach (Node node in graph.Nodes)
        //    {
        //        EditorGUILayout.LabelField("Node " + count++, node.Id);
        //        //EditorGUILayout.ObjectField(edgeBeh, typeof(Behaviour_Edge2D), GUILayout.ExpandWidth(true));
        //    }
        //    CustomGUIUtils.EndGroup();
        //}


        //if (edgesFoldout = EditorGUILayout.Foldout(edgesFoldout, "Edges (" + graph.Edges.Count + ")"))
        //{
        //    CustomGUIUtils.BeginGroup();
        //    int count = 1;
        //    foreach (Edge edge in graph.Edges)
        //    {
        //        EditorGUILayout.LabelField("Edge " + count++, edge.Id);
        //        //EditorGUILayout.ObjectField(edgeBeh, typeof(Behaviour_Edge2D), GUILayout.ExpandWidth(true));
        //    }
        //    CustomGUIUtils.EndGroup();
        //}

        if (nodesFoldout = EditorGUILayout.Foldout(nodesFoldout, "Node Behaviours (" + graphBehaviour.NodeBehaviours.Count + ")"))
        {
            CustomGUIUtils.BeginGroup();
            int count = 1;
            foreach (Node_Behaviour nodeBehaviour in graphBehaviour.NodeBehaviours)
            {
                EditorGUILayout.LabelField("NodeBehaviour " + count++, nodeBehaviour.Node.Id);
                //EditorGUILayout.ObjectField(edgeBeh, typeof(Behaviour_Edge2D), GUILayout.ExpandWidth(true));
            }
            CustomGUIUtils.EndGroup();
        }


        if (edgesFoldout = EditorGUILayout.Foldout(edgesFoldout, "Edge Behaviours (" + graphBehaviour.EdgeBehaviours.Count + ")"))
        {
            CustomGUIUtils.BeginGroup();
            int count = 1;
            foreach (Edge_Behaviour edgeBehaviour in graphBehaviour.EdgeBehaviours)
            {
                EditorGUILayout.LabelField("EdgeBehaviour " + count++, edgeBehaviour.Edge.Id);
                //EditorGUILayout.ObjectField(edgeBeh, typeof(Behaviour_Edge2D), GUILayout.ExpandWidth(true));
            }
            CustomGUIUtils.EndGroup();
        }



        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        // Draw the DefaultInspector for this Behvaiour
        showDefaultInspector = this.DrawDefaultInspectorFoldout(graphBehaviour, showDefaultInspector);


        if (GUI.changed)
            EditorUtility.SetDirty(graphBehaviour);
    }

}
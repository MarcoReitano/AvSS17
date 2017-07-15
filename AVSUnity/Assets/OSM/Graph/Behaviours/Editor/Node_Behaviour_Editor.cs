using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(Node_Behaviour))]
public class Node_Behaviour_Editor : Editor
{

    [SerializeField]
    private Node node;
    [SerializeField]
    private Node_Behaviour nodeBehaviour;
    [SerializeField]
    private Object[] nodes;
    [SerializeField]
    private List<Node_Behaviour> nodeBehaviours;
    [SerializeField]
    private Vector3 oldPosition = Vector3.zero;



    [SerializeField]
    private static bool showDefaultInspector = false;
    [SerializeField]
    private bool advancedGizmoSettings = true;
    [SerializeField]
    private static bool edgesFoldout = true;
    [SerializeField]
    private static bool arrivalsFoldout = true;
    [SerializeField]
    private static bool departuresFoldout = true;
    [SerializeField]
    private static bool listenerFoldout = true;



    public void Awake()
    {
        // the single target
        nodeBehaviour = target as Node_Behaviour;
        node = nodeBehaviour.Node;

        // multiple targets
        nodeBehaviours = new List<Node_Behaviour>();
        nodes = targets;
        foreach (Object item in nodes)
        {
            Node_Behaviour go = (Node_Behaviour)item;
            nodeBehaviours.Add(go);
        }
    }

    public List<Vector3> intersections = new List<Vector3>();

    public void OnSceneGUI()
    {
        UnityEngine.Event currentEvent = UnityEngine.Event.current;

        //NodeUtils.DrawCircle(node.Position, 3f);

        // Delete the Node
        if (InputHelpers.Pressed(KeyCode.D))
        {
            nodeBehaviour.Node.Delete();
            return;
        }


        /*****************-  Position-Changes -************************************************************/
        // Update Positions for single or multi-select
        if (nodeBehaviour.transform.position != oldPosition)
        {
            oldPosition = nodeBehaviour.transform.position;

            foreach (Node_Behaviour go in nodeBehaviours)
            {
                NodeUtils.DrawAnglePieGizmo(go.Node);
                if (Event.current.control)
                {
                    go.transform.position = go.transform.position.SnapToGrid();
                    go.Node._position = go.transform.position;
                }
                else
                {
                    go.Node._position = go.transform.position;
                }

                this.nodeBehaviour.GraphBehaviour.UpdateEdgeIntersections();

            }
            //// fire change-events


            foreach (Node_Behaviour go in nodeBehaviours)
            {
                //Debug.Log(go.name + "OnNodePosition Node2D_Editor");
                go.Node.OnNodeChanged();
            }

        }
        /*****************************************************************************************************/

        // Get the mousepointer on XZ-plane and draw a line
        Vector3 worldPoint = InputHelpers.GetXZPlaneCollisionInEditorFixedHeight(0f, currentEvent); //InputHelpers.GetXZPlaneCollisionInEditor(currentEvent);

        if (Event.current.control)
            worldPoint = worldPoint.SnapToGrid();

        Handles.color = Color.green;
        Handles.DrawLine(nodeBehaviour.transform.position, worldPoint);



        if (InputHelpers.ClickedLeft())
        {
            //if()

            // if another Node is within distance, connect the two nodes with a new edge
            Node_Behaviour foundNode = nodeBehaviour.GraphBehaviour.GetNodeInDistance(worldPoint, 1f);

            if (foundNode != null)
            {
                if (foundNode != nodeBehaviour)
                {
                    // Create a new EdgeBehaviour with the given nodes
                    Edge_Behaviour newEdgeBehaviour = nodeBehaviour.GraphBehaviour.CreateEdgeBehaviour(nodeBehaviour, foundNode);

                    foundNode.gameObject.SelectInEditor();

                    //newEdgeBehaviour.Edge.OnFromToChanged();
                    //}
                    //if (currentEvent.control || currentEvent.shift)
                    //{
                    //Node_Behaviour.combineWithLastSelectedNode = true;
                    //currentEvent.Use();
                }
            }
            else
            {
                Vector3 pointOnEdge;
                float pointOnEdgeParameter;
                Edge_Behaviour foundEdge = nodeBehaviour.GraphBehaviour.GetEdgeInDistanceWithPoint(worldPoint, 1f, out pointOnEdge, out pointOnEdgeParameter);
                if (foundEdge != null)
                {
                    Debug.Log("Found Edge: " + foundEdge.name + "  hit at t = " + pointOnEdgeParameter + " at position " + pointOnEdge);

                    // Create new Node at the cutting-point on the Edge
                    Node_Behaviour newNodeBehaviour = nodeBehaviour.GraphBehaviour.CreateNodeBehaviour(pointOnEdge);

                    // Create new Edge for first part
                    Edge_Behaviour cutEdge1 = nodeBehaviour.GraphBehaviour.CreateEdgeBehaviour(foundEdge.FromBehaviour, newNodeBehaviour);

                    // Create new Edge for second part
                    Edge_Behaviour cutEdge2 = nodeBehaviour.GraphBehaviour.CreateEdgeBehaviour(newNodeBehaviour, foundEdge.ToBehaviour);

                    if (foundEdge.FromBehaviour != nodeBehaviour && foundEdge.ToBehaviour != nodeBehaviour)
                    {
                        // Create new Edge from the lastNode (this) to the new Node
                        Edge_Behaviour newEdgeBehaviour = nodeBehaviour.GraphBehaviour.CreateEdgeBehaviour(nodeBehaviour, newNodeBehaviour);
                        //Debug.Log("#############  Done creating a new Wall  ##################");
                    }

                    foundEdge.Edge.Delete();

                    newNodeBehaviour.Node.OnNodeChanged();
                    newNodeBehaviour.gameObject.SelectInEditor();
                }
                else //if (nodeBehaviour.GraphBehaviour != null)
                {
                    //Debug.Log("#############  Start creating a new Wall  ##################");
                    Node_Behaviour newNodeBehaviour = nodeBehaviour.GraphBehaviour.CreateNodeBehaviour(worldPoint);
                    Edge_Behaviour newEdgeBehaviour = nodeBehaviour.GraphBehaviour.CreateEdgeBehaviour(nodeBehaviour, newNodeBehaviour);
                    //Debug.Log("#############  Done creating a new Wall  ##################");

                    newNodeBehaviour.gameObject.SelectInEditor();

                    currentEvent.Use();
                }
            }
        }
        HandleUtility.Repaint();


        //if (InputHelpers.MouseUp())
        //{
        //    Debug.Log("RoomRecognition");
        //    ////this is sooooo NOT good....
        //    foreach (Wall wall in node.Graph.Iem.Prefab.Walls)
        //        wall.RoomRecognition();
        //}

        //if (GUI.changed)
        //    EditorUtility.SetDirty(nodeBehaviour);
    }




    public override void OnInspectorGUI()
    {
        GUILayout.Label("Behaviour_Node2D-Editor", EditorStyles.boldLabel);

        if (Selection.gameObjects.Length >= 2)
        {
            if (GUILayout.Button("Merge Nodes"))
            {
                GameObject node1 = Selection.gameObjects[0];
                Node_Behaviour node1Behaviour = node1.GetComponent<Node_Behaviour>();
                Node node1Node = node1Behaviour.Node;

                List<GameObject> mergeNodesGOs = new List<GameObject>();
                List<Node> combineNodes = new List<Node>();
                for (int i = 1; i < Selection.gameObjects.Length; i++)
                {
                    GameObject node2 = Selection.gameObjects[1];
                    Node_Behaviour node2Behaviour = node2.GetComponent<Node_Behaviour>();
                    Node node2Node = node2Behaviour.Node;
                    combineNodes.Add(node2Node);
                    mergeNodesGOs.Add(node2);
                }

                node1Node.MergeWith(combineNodes.ToArray());
                foreach (GameObject go in mergeNodesGOs)
                    DestroyImmediate(go);

            }
        }


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        //// Get Position from changes in the VectorField in Inspection
        Vector3 oldPosition = node.Position;
        Vector3 newPosition = EditorGUILayout.Vector3Field("Position", node.Position);
        if (oldPosition != newPosition)
        {
            Debug.Log("Auslöser Node2D-Editor-OnInspectorGUI");
            node.Position = newPosition;
            nodeBehaviour.transform.position = newPosition;
        }


        if (nodeBehaviour != null && node != null)
        {
            EditorGUILayout.Separator();
            if (edgesFoldout = EditorGUILayout.Foldout(edgesFoldout, "Edges"))
            {
                CustomGUIUtils.BeginGroup();

                if (arrivalsFoldout = EditorGUILayout.Foldout(arrivalsFoldout, "Arrivals (" + node.ArrivalDegree.ToString() + ")"))
                {
                    CustomGUIUtils.BeginGroup();
                    int count = 0;
                    foreach (Edge edge in node.Arrivals)
                    {
                        EditorGUILayout.LabelField("Arrival " + count++, edge.Id);
                    }
                    CustomGUIUtils.EndGroup();
                }

                if (departuresFoldout = EditorGUILayout.Foldout(departuresFoldout, "Departures (" + node.DepartureDegree.ToString() + ")"))
                {
                    CustomGUIUtils.BeginGroup();
                    int count = 0;
                    foreach (Edge edge in node.Departures)
                    {
                        EditorGUILayout.LabelField("Departure " + count++, edge.Id);
                    }
                    CustomGUIUtils.EndGroup();
                }

                if (EditorGUILayout.Foldout(listenerFoldout, "Listeners"))
                {
                    CustomGUIUtils.BeginGroup();
                    int count = 0;

                    foreach (string name in node.PrintEventHandlers())
                    {
                        EditorGUILayout.LabelField("" + count++, name);
                    }
                    CustomGUIUtils.EndGroup();
                }

                CustomGUIUtils.EndGroup();
            }
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (advancedGizmoSettings = EditorGUILayout.Foldout(advancedGizmoSettings, "Behaviour_Edge2D-Advanced-Gizmo-Settings"))
        {
            CustomGUIUtils.BeginGroup();
            Node_Behaviour.NodeGizmoSize = (Node_Behaviour.GizmoSize)EditorGUILayout.EnumPopup("GizmoSize", Node_Behaviour.NodeGizmoSize);
            Node_Behaviour.DrawLabels = EditorGUILayout.Toggle("Show Label", Node_Behaviour.DrawLabels);
            nodeBehaviour.DrawHelpLinesGizmo = EditorGUILayout.Toggle("Show Help-Lines", nodeBehaviour.DrawHelpLinesGizmo);
            nodeBehaviour.DrawSecondLevelHelpLinesGizmo = EditorGUILayout.Toggle("Show SecondLevel Help-Lines", nodeBehaviour.DrawSecondLevelHelpLinesGizmo);

            Node_Behaviour.DrawAnglePieGizmoMaster = EditorGUILayout.Toggle("Draw Angle-Pie Gizmo", Node_Behaviour.DrawAnglePieGizmoMaster);

            if (Node_Behaviour.DrawAnglePieGizmoMaster)
            {
                CustomGUIUtils.BeginGroup();
                nodeBehaviour.DrawAnglePieGizmo = EditorGUILayout.Toggle("Draw Angle-Pie Gizmo", nodeBehaviour.DrawAnglePieGizmo);
                if (nodeBehaviour.DrawAnglePieGizmo)
                {
                    Node_Behaviour.DrawAnglePieGizmoDynamic = EditorGUILayout.Toggle("Dynamic Radius", Node_Behaviour.DrawAnglePieGizmoDynamic);

                    if (!Node_Behaviour.DrawAnglePieGizmoDynamic)
                        nodeBehaviour.AnglePieGizmoRadius = EditorGUILayout.Slider("Radius", nodeBehaviour.AnglePieGizmoRadius, 0f, 10f);
                }
                CustomGUIUtils.EndGroup();
            }
            CustomGUIUtils.EndGroup();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        // Draw the DefaultInspector for this Behvaiour
        showDefaultInspector = this.DrawDefaultInspectorFoldout(nodeBehaviour, showDefaultInspector);

        if (GUI.changed)
            EditorUtility.SetDirty(nodeBehaviour);
    }
}
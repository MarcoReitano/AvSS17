using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[CustomEditor(typeof(Edge_Behaviour))]
public class Edge_Behaviour_Editor : Editor
{

    [SerializeField]
    private Edge_Behaviour edgeBehaviour;

    [SerializeField]
    private Edge edge;

    [SerializeField]
    private bool showDefaultInspector = false;
    [SerializeField]
    private bool advancedGizmoSettings = true;


    public void Awake()
    {
        edgeBehaviour = target as Edge_Behaviour;
        edge = edgeBehaviour.Edge;
        if (edge != null)
            edgeBehaviour.transform.position = edge.GetMidPoint();
        //else
        //  DestroyImmediate(edgeBehaviour.gameObject);
    }


    public void OnSceneGUI()
    {


        if (edge != null && !edgeBehaviour.gameObject.IsDestroyed())
        {
            UnityEngine.Event currentEvent = UnityEngine.Event.current;

            // Just in case that the values are changed in the editor
            if ((edge.From != null) && (edge.To != null))
            {
                if (currentEvent.type != EventType.mouseDrag)
                {
                    Vector3 handlePosition = edge.Position;

                    Vector3 newPosition = edgeBehaviour.transform.position;
                    if (newPosition != handlePosition)
                    {
                        edge.Position = newPosition;
                        edgeBehaviour.transform.position = edge.Position;
                        this.edgeBehaviour.GraphBehaviour.UpdateEdgeIntersections();
                    }
                }
            }

            // Delete edge if D-Key is pressed
            if (InputHelpers.Pressed(KeyCode.D) && edgeBehaviour != null)
            {
                edgeBehaviour.Edge.Delete();
                return;
            }

            // Worked: but only for node/edge-layer  --> wall-endings have to be changed as well...
            // -->  maybe with teh graphEvent FromToChanged in the Wall
            //// Switch ends on DoubleClick
            //if (InputHelpers.DoubleLeftClick())
            //    edge.SwitchEnds();

            if (InputHelpers.DoubleLeftClicked())
            {
                // Get the mousepointer on XZ-plane and draw a line
                Vector3 worldPoint = InputHelpers.GetXZPlaneCollisionInEditor(currentEvent);


                float pointOnEdgeParameter;
                Vector3 pointOnEdge = MathUtils.GetMinDistancePointOnLine(
                    edgeBehaviour.Edge.From.Position,
                    edgeBehaviour.Edge.To.Position,
                    worldPoint,
                    out pointOnEdgeParameter);


                // Create new Node at the cutting-point on the Edge
                Node_Behaviour newNodeBehaviour = edgeBehaviour.GraphBehaviour.CreateNodeBehaviour(pointOnEdge);

                // Create new Edge for first part
                Edge_Behaviour cutEdge1 = edgeBehaviour.GraphBehaviour.CreateEdgeBehaviour(edgeBehaviour.FromBehaviour, newNodeBehaviour);

                // Create new Edge for second part
                Edge_Behaviour cutEdge2 = edgeBehaviour.GraphBehaviour.CreateEdgeBehaviour(newNodeBehaviour, edgeBehaviour.ToBehaviour);


                //HandleUtility.Repaint();
                edgeBehaviour.Edge.Delete();
                Debug.Log("Deleted old Edge");

                newNodeBehaviour.Node.OnNodeChanged();
                Debug.Log("NewNode ChangedEvent");

                UnityEngine.Object[] objects = new UnityEngine.Object[1];
                objects[0] = newNodeBehaviour.gameObject;
                Selection.objects = objects;
                Debug.Log("After selecting the new node");

                Undo.CreateSnapshot();
                Debug.Log("Create Snapshot");
                HandleUtility.Repaint();
                Debug.Log("Repaint");
                return;
            }

            if (GUI.changed)
                EditorUtility.SetDirty(edgeBehaviour);
        }
    }




    public override void OnInspectorGUI()
    {


        if (GUILayout.Button("Delete Edge"))
        {
            //Node oldFromNode = edgeBehaviour.Edge.From;
            //Node oldToNode = edgeBehaviour.Edge.To;
            edgeBehaviour.Delete();
            //oldFromNode.OnNodeChanged();
            //oldToNode.OnNodeChanged();
        }




        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        if (advancedGizmoSettings = EditorGUILayout.Foldout(advancedGizmoSettings, "Edge-Advanced-Gizmo-Settings"))
        {
            CustomGUIUtils.BeginGroup();
            Edge_Behaviour.NodeGizmoSize = (Edge_Behaviour.GizmoSize)EditorGUILayout.EnumPopup("GizmoSize", Edge_Behaviour.NodeGizmoSize);
            Edge_Behaviour.DrawLabel = EditorGUILayout.Toggle("Show Label", Edge_Behaviour.DrawLabel);
            Edge_Behaviour.DrawLength = EditorGUILayout.Toggle("Show Length", Edge_Behaviour.DrawLength);

            CustomGUIUtils.EndGroup();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        // Draw the DefaultInspector for this Behvaiour
        showDefaultInspector = this.DrawDefaultInspectorFoldout(edgeBehaviour, showDefaultInspector);

        if (GUI.changed)
            EditorUtility.SetDirty(edgeBehaviour);
    }
}
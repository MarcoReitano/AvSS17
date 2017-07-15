using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Text;


public class Node_Behaviour : MonoBehaviour
{
    // The Parent
    [SerializeField]
    private Graph_Behaviour graphBehaviour;
    public Graph_Behaviour GraphBehaviour
    {
        get { return graphBehaviour; }
        set { graphBehaviour = value; }
    }

    // The Node
    [SerializeField]
    private Node node;
    public Node Node
    {
        get { return node; }
        set
        {
            //node = value;
            //transform.position = node.Position;
            //this.onGraphLayerChanged = new EventHandler(OnNodePositionChanged);
            //node.ChangedGraphLayer += onGraphLayerChanged;
            ////GL_RendererOnCamera.DrawQuads += new EventHandler(Draw2DNodeGL);

            if (node != null)
            {
                this.node.ChangedGraphLayer -= onGraphLayerChanged;
            }

            node = value;

            if (value != null)
            {
                if (node != null)
                {
                    this.node.ChangedGraphLayer += onGraphLayerChanged;
                    this.transform.position = node.Position;
                    //GL_RendererOnCamera.DrawLines += new EventHandler(DrawEdgeGL);
                }
            }
            else if (value == null)
            {
                Delete();
            }
        }
    }

    public void InitializeBehaviour(Graph_Behaviour graphBehaviour, Node node)
    {
        this.onGraphLayerChanged = new EventHandler(OnGraphEvent);
        this.GraphBehaviour = graphBehaviour;
        this.Node = node;
    }


    public static bool combineWithLastSelectedNode = false;


    #region Observe Others
    private EventHandler onGraphLayerChanged;
    private void OnGraphEvent(System.Object sender, EventArgs e)
    {
        transform.position = node.Position;
        GraphEventArgs graphEventArgs = (GraphEventArgs)e;

        switch (graphEventArgs.GraphEvent)
        {
            case GraphEventType.NodeChanged:
                break;
            case GraphEventType.NodeDeleted:
                if (graphEventArgs.Origin == this.node)
                {
                    this.Node = null;
                    //Delete();
                }
                break;
            case GraphEventType.EdgeChanged:
                break;
            case GraphEventType.EdgeFromChanged:
                break;
            case GraphEventType.EdgeToChanged:
                break;
            case GraphEventType.EdgeDeleted:
                break;
            case GraphEventType.EdgeEndsSwitched:
                break;
            default:
                break;
        }


    }
    #endregion

    #region Gizmo-related Methods
    public enum GizmoSize
    {
        SMALL,
        MEDIUM,
        LARGE,
        EXTRA_LARGE
    }

    private static string gizmoFileName = "UnityGraph/NodeWhite16.png";
    private static GizmoSize gizmoSize = GizmoSize.MEDIUM;
     
    [SerializeField]
    public static bool DrawLabels = true;
    [SerializeField]
    public bool DrawAnglePieGizmo = true; 
    [SerializeField]
    public static bool DrawAnglePieGizmoMaster = false;
    [SerializeField]
    public static bool DrawAnglePieGizmoDynamic = false;
    [SerializeField]
    public bool DrawHelpLinesGizmo = false;
    [SerializeField]
    public bool DrawSecondLevelHelpLinesGizmo = false;


    [SerializeField]
    private float anglePieGizmoRadius = 1f;
    public float AnglePieGizmoRadius
    {
        get { return anglePieGizmoRadius; }
        set { anglePieGizmoRadius = value; }
    }

    public static GizmoSize NodeGizmoSize
    {
        get { return Node_Behaviour.gizmoSize; }
        set
        {
            Node_Behaviour.gizmoSize = value;
            switch (gizmoSize)
            {
                case GizmoSize.SMALL:
                    gizmoFileName = "UnityGraph/NodeWhite8.png";
                    break;
                case GizmoSize.MEDIUM:
                    gizmoFileName = "UnityGraph/NodeWhite16.png";
                    break;
                case GizmoSize.LARGE:
                    gizmoFileName = "UnityGraph/NodeWhite32.png";
                    break;
                case GizmoSize.EXTRA_LARGE:
                    gizmoFileName = "UnityGraph/NodeWhite64.png";
                    break;
                default:
                    break;
            }
        }
    }


    public Color color = XKCDColors.AcidGreen;
    public float width = 0.5f;

    public void Draw2DNodeGL(System.Object sender, EventArgs e)
    {
        //GL.Color(this.color);
        
        //GL.TexCoord2(0, 0); GL.Vertex(this.transform.position - new Vector3(-width, 0, -width));
        //GL.TexCoord2(1, 0); GL.Vertex(this.transform.position - new Vector3(-width, 0, +width));
        //GL.TexCoord2(0, 1); GL.Vertex(this.transform.position - new Vector3(+width, 0, +width));
        //GL.TexCoord2(1, 1); GL.Vertex(this.transform.position - new Vector3(+width, 0, -width));
    }


    /// <summary>
    /// 
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, gizmoFileName, false);
        
        if (DrawAnglePieGizmoMaster)
        {
            if (DrawAnglePieGizmo)
            {
                if (DrawAnglePieGizmoDynamic)
                    NodeUtils.DrawAnglePieGizmo(this.node);
                else
                    NodeUtils.DrawAnglePieGizmo(this.node, anglePieGizmoRadius);
            }
        }

#if UNITY_EDITOR
        if (DrawLabels)
            Handles.Label(transform.position, this.name);

        if(DrawHelpLinesGizmo)
            DrawHelpLines(this.node, Color.yellow, false);

        if(DrawSecondLevelHelpLinesGizmo)
            DrawSecondLevelHelpLines(this.node);
#endif
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        //if (!this.drawAnglePieGizmo)
        //    DrawAnglePieGizmo();

        //foreach (Edge edge in this.node.Arrivals)
        //    edge.From..DrawAnglePieGizmo();

        //foreach (Edge edge in departures)
        //    edge.to.DrawAnglePieGizmo();

    }
    #endregion



    private static float helpLineLength = 10f;
    
    public void DrawHelpLines()
    {
        DrawHelpLines(this.node, Color.yellow, false);
        DrawSecondLevelHelpLines(this.node);
    }

    public static void DrawHelpLines(Node node, Color color, bool secondLevel)
    {
#if UNITY_Editor
        foreach (Edge edge in node.Edges)
        {
            Handles.color = color;
            Handles.DrawLine(node.Position, node.Position + edge.Vector.normalized * helpLineLength);
            Handles.DrawLine(node.Position, node.Position - edge.Vector.normalized * helpLineLength);

            if (!secondLevel)
            {
                Handles.color = Color.gray;
                Handles.DrawLine(node.Position, node.Position + edge.OrthoNormalVector * helpLineLength);
                Handles.DrawLine(node.Position, node.Position - edge.OrthoNormalVector * helpLineLength);
            }

        }
#endif
    }


    public void DrawSecondLevelHelpLines()
    {
        DrawSecondLevelHelpLines(this.node);
    }

    public static void DrawSecondLevelHelpLines(Node node)
    {
        foreach (Edge edge in node.Edges)
        {
            if (edge.From == node)
                Node_Behaviour.DrawHelpLines(edge.To, Color.green, true);
            else
                Node_Behaviour.DrawHelpLines(edge.From, Color.green, true);
        }
    }


    // Use this for initialization
    void Start()
    {
        
    }

    void OnGUI()
    {

    }

    // Update is called once per frame
    void Update()
    {
         
    }

    
    public bool Delete()
    {
        Debug.Log(this.name + " Deleted");
        if (this.node != null)
        {
            //Debug.Log("Node Behaviour  node!=null");
            //this.node = null;
            this.node.Delete();
            return true;
        }
        else
        {
            //Debug.Log("Node Behaviour  node==null");
            this.graphBehaviour.RemoveNodeBehaviour(this);
            //this.edge = null;
            this.graphBehaviour = null;
#if UNITY_EDITOR
            if (this != null)
                GameObject.DestroyImmediate(this.gameObject);

#else
            if(this != null)
                GameObject.Destroy(this.gameObject);
#endif
            return false;
        }

    }

}

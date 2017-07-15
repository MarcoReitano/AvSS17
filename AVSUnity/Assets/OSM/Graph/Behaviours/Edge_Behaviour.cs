using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Globalization;
using System.Collections.Generic;


public class Edge_Behaviour : MonoBehaviour
{
    [SerializeField]
    private Graph_Behaviour graphBehaviour;
    public Graph_Behaviour GraphBehaviour
    {
        get { return graphBehaviour; }
        set { graphBehaviour = value; }
    }

    private EventHandler drawEdgeGL;

    [SerializeField]
    private Edge edge;
    public Edge Edge
    {
        get { return edge; }
        set
        {
            if (edge != null)
                this.edge.Changed -= onGraphEvent;

            edge = value;

            if (value != null)
            {
                if (edge != null)
                {
                    this.edge.Changed += onGraphEvent;
                    this.transform.position = edge.Position;
                    //GL_RendererOnCamera.DrawLines += new EventHandler(DrawEdgeGL);
                }
            }
            else if (value == null)
                Delete();
        }
    }

    public Node_Behaviour fromBehaviour;
    public Node_Behaviour FromBehaviour { 
        get { return fromBehaviour;}
        set {
            this.fromBehaviour = value;
        }
    }

    public Node_Behaviour toBehaviour;
    public Node_Behaviour ToBehaviour
    {
        get { return toBehaviour; }
        set
        {
            this.toBehaviour = value;
        }
    }



    public void InitializeBehaviour(Graph_Behaviour graphBehaviour, Edge edge, Node_Behaviour fromBehaviour, Node_Behaviour toBehaviour)
    {
        this.onGraphEvent = new EventHandler(OnGraphEvent);
        this.GraphBehaviour = graphBehaviour;
        this.Edge = edge;
        this.fromBehaviour = fromBehaviour;
        this.ToBehaviour = toBehaviour;
    }


    public bool Delete()
    {
        //Debug.Log(this.name + " Deleted");
        if (this.edge != null)
        {
            this.edge.Delete();
            return true;
        }
        else 
        {
            this.graphBehaviour.RemoveEdgeBehaviour(this);
            this.graphBehaviour = null;
#if UNITY_EDITOR
            if(this != null)
                GameObject.DestroyImmediate(this.gameObject);

#else
            if(this != null)
                GameObject.Destroy(this.gameObject);
#endif
            return false;
        }
    }

  
    public static Edge_Behaviour GetEdgeBehaviour(Edge edge)
    {
        Edge_Behaviour[] gos = GameObject.FindObjectsOfType(typeof(Edge_Behaviour)) as Edge_Behaviour[];

        foreach (Edge_Behaviour go in gos)
        {
            if (go.edge == edge)
                return go;
        }

        return null;
    }


    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }



    public enum GizmoSize
    {
        SMALL,
        MEDIUM,
        LARGE,
        EXTRA_LARGE
    }


    [SerializeField]
    private bool drawGizmos = true;

    [SerializeField]
    private bool DrawArrow = true;

    private static string gizmoFileName = "UnityGraph/EdgeWhite8.png";
    private static GizmoSize gizmoSize = GizmoSize.SMALL;
    public static GizmoSize NodeGizmoSize
    {
        get { return Edge_Behaviour.gizmoSize; }
        set
        {
            Edge_Behaviour.gizmoSize = value;
            switch (gizmoSize)
            {
                case GizmoSize.SMALL:
                    gizmoFileName = "UnityGraph/EdgeWhite8.png";
                    break;
                case GizmoSize.MEDIUM:
                    gizmoFileName = "UnityGraph/EdgeWhite16.png";
                    break;
                case GizmoSize.LARGE:
                    gizmoFileName = "UnityGraph/EdgeWhite32.png";
                    break;
                case GizmoSize.EXTRA_LARGE:
                    gizmoFileName = "UnityGraph/EdgeWhite64.png";
                    break;
                default:
                    break;
            }
        }
    }

    #region Observe Others
    private EventHandler onGraphEvent;
    private void OnGraphEvent(System.Object sender, EventArgs e)
    {
        GraphEventArgs args = (GraphEventArgs)e;

        //Debug.Log("Edge-Behaviour " + gameObject.name +  "  \n" + args.ToString());
        switch (args.GraphEvent)
        {
            //case GraphEventType.NodeChanged:
            //    Debug.Log(this.name + " EdgeBehaviour: NodeChanged (sender" + ((Edge)sender).ToString() + ")");
            //    break;
            //case GraphEventType.NodeDeleted:
            //    Debug.Log(this.name + " EdgeBehaviour: NodeDeleted (sender" + ((Edge)sender).ToString() + ")");
            //    break;
            case GraphEventType.EdgeChanged:
                //Debug.Log(this.name + " EdgeBehaviour: EdgeChanged (sender" + ((Edge)sender).ToString() + ")");
                this.transform.position = edge.Position;
                break;
            case GraphEventType.EdgeFromChanged:
                //Debug.Log(this.name + " EdgeBehaviour: EdgeFromChanged (sender" + ((Edge)sender).ToString() + ")");
                this.fromBehaviour = graphBehaviour.GetNodeBehaviourByID(this.edge.From.Id);
                break;
            case GraphEventType.EdgeToChanged:
                //Debug.Log(this.name + " EdgeBehaviour: EdgeToChanged (sender" + ((Edge)sender).ToString() + ")");
                this.toBehaviour = graphBehaviour.GetNodeBehaviourByID(this.edge.To.Id);
                break;
            case GraphEventType.EdgeDeleted:
                //Debug.Log(this.name + " EdgeBehaviour: EdgeDeleted (sender" + ((Edge)sender).ToString() + ")");
                // TODO: wenn andere auf das Edge-Behaviour hören, muss HIER zuerst weitergefeuert werden.
                this.Edge = null;
                return;
            default:
                break;
        }
    } 
    #endregion


    public static bool DrawLabel = true;
    public static bool DrawLength = false;

    public Color color = new Color(0 / 255f, 153 / 255f, 204 / 255f);

    /// <summary>
    /// 
    /// </summary>
    public void OnDrawGizmos()
    {
        if (this.edge != null)
        {
            if ((this.edge.From != null) && (this.edge.To != null))
            {
                if (drawGizmos)
                {
                    Gizmos.color = new Color(0 / 255f, 153 / 255f, 204 / 255f);
                    Gizmos.DrawLine(this.edge.From.Position, this.edge.To.Position);

#if UNITY_EDITOR
                    if (DrawArrow)
                    {
                        Handles.color = Color.green;
                        float size = Mathf.Clamp(HandleUtility.GetHandleSize(edge.Position), 0, this.edge.Vector.magnitude);
                        
                        if(this.Edge.Vector != Vector3.zero)
                            Handles.ArrowCap(0, this.edge.Position - (this.edge.Vector.normalized / 2) * size, Quaternion.LookRotation(this.edge.Vector), size);
                    }
#endif
                    Gizmos.DrawLine(this.edge.From.Position, this.edge.To.Position);

                    Gizmos.DrawIcon(edge.Position, gizmoFileName, false);
#if UNITY_EDITOR
                    string label = string.Empty;

                    if (DrawLabel || DrawLength)
                    {
                        if (DrawLabel)
                            label = this.edge.Id + "\n";

                        if (DrawLength)
                            label += this.edge.Vector.magnitude.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "m";

                        Handles.Label(edge.Position, label);
                    }
#endif
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DrawEdgeGL(System.Object sender, EventArgs e)
    {
        GL.Color(this.color);
        GL.Vertex(this.edge.From.Position);
        GL.Vertex(this.edge.To.Position);
    }
}

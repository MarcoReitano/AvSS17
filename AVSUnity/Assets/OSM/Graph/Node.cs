using System;
using System.Collections.Generic;
using System.Linq;
//using ExtensionMethods;
using UnityEngine;
using System.Text;



public class Node : IHasID
{
    #region Properties
    // The Parent
    [SerializeField]
    private Graph graph;
    public Graph Graph
    {
        get { return graph; }
        set { graph = value; }
    }

    #region Properties (Schema)
    [SerializeField]
    private string id;
    public string Id
    {
        get { return id; }
        set { id = value; }
    }

    // The position
    [SerializeField]
    public Vector3 _position;
    public Vector3 Position
    {
        get { return _position; }
        set
        {
            _position = value;
            // Fire position-change to the observers
            OnNodeChanged();
        }
    }
    #endregion // Properties (Schema)

    #region Derived Properties
    // The union of the arrivals and departures --> all edges incoming and outgoing
    [SerializeField]
    private List<Edge> edges;
    public List<Edge> Edges
    {
        get
        {
            this.edges = GetAllEdges(true);
            return this.edges;
        }
    }


    // The Degree of the node, meaning the overall count of adjacent edges
    public int Degree { get { return ArrivalDegree + DepartureDegree; } }

    // In the node arriving edges
    [SerializeField]
    private List<Edge> arrivals;
    public List<Edge> Arrivals { get { return arrivals; } }

    // The number of arriving edges
    public int ArrivalDegree { get { return Arrivals.Count; } }

    // From the node departing/outgoing edges
    [SerializeField]
    private List<Edge> departures;
    public List<Edge> Departures { get { return departures; } }

    // The number of departing edges
    public int DepartureDegree { get { return Departures.Count; } }
    #endregion // Derived Properties
    #endregion // Properties

    #region Constructors
    public Node(Graph graph)
    {
        this.graph = graph;
        this.edges = new List<Edge>();
        this.arrivals = new List<Edge>();
        this.departures = new List<Edge>();
    }
       

    public Node(Graph graph, Vector3 position)
        : this(graph)
    {
        this.id = IDUtils.GetUniqueID("Node", string.Empty, this.graph.Nodes, 10000);
        this._position = position;
    }

    public Node(Graph graph, string id, Vector3 position)
        : this(graph, position)
    {
        this.id = id;
    }
    #endregion //Constructors

    #region Destructor
    public Node Delete()
    {
        //Debug.Log("Node.Delete() --> ");
        OnNodeDeleted();

        if (this.Edges.Count > 0)
        {
            //Debug.Log("Node.Delete() --> Perform ClearEdges");
            ClearEdges();
        }

        graph.RemoveNode(this, false);

        return this;
    }
    #endregion //Destructor

    #region Methods

    public void Refresh()
    {
        this.edges = GetAllEdges(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<Edge> GetAllEdges(bool sorted)
    {
        // Declare and instatiate new edges-list
        this.edges = new List<Edge>();

        // Add all arrivals and departures to the new list
        this.edges.AddRange(Arrivals);
        this.edges.AddRange(Departures);

        // Sort the list clockwise to the Z-Axis 
        if (sorted)
            this.edges = SortEdges(this.edges, this);

        return this.edges;
    }



    #region Arrival- and -Departure-Methods
    #region Arrival-Methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public bool AddArrival(Edge edge)
    {
        return AddArrival(edge, true);
    }
    public bool AddArrival(Edge edge, bool fireEvent)
    {
        if (edge != null)
        {
            if (edge.From != null)
            {

                // This bit me in the ass...  works perfect for the edge-creation, but NOT for Merging Nodes
                //if (!this.Arrivals.Contains(edge)
                //    && this.Graph.GetEdge(edge.From, edge.To) == null
                //    && this.Graph.GetEdge(edge.To, edge.From) == null)


                if (!this.Arrivals.Contains(edge))
                {
                    this.Arrivals.Add(edge);
                    edge.Changed += OnEdgeEvent;
                    if (fireEvent)
                        OnNodeChanged();
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public bool RemoveArrival(Edge edge)
    {
        return RemoveArrival(edge, true);
    }

    public bool RemoveArrival(Edge edge, bool fireEvent)
    {
        edge.Changed -= OnEdgeEvent;
        bool success = Arrivals.Remove(edge);
        if (success)
        {
            RefreshEdges();
            if (fireEvent)
                OnNodeChanged();
        }

        return success;
    }

    /// <summary>
    /// 
    /// </summary>
    public void ClearArrivals()
    {
        if (this.Arrivals.Count == 0)
            return;

        List<Edge> concurrentArrivals = new List<Edge>(Arrivals);

        foreach (Edge deleteEdge in concurrentArrivals)
        {
            Arrivals.Remove(deleteEdge);
            RefreshEdges();
            deleteEdge.Delete();
        }

        this.arrivals = new List<Edge>();
        RefreshEdges();
    }
    #endregion // Arrival-Methods

    #region Departure-Methods


    /// <summary>
    /// Add an outgoing edge to the departures-list
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public bool AddDeparture(Edge edge)
    {
        //Debug.Log(this.id + "  AddDeparture: " + edge.ToString());
        return AddDeparture(edge, true);
    }

    public bool AddDeparture(Edge edge, bool fireEvent)
    {

        if (edge != null)
        {
            //Debug.Log(this.id + "  AddDeparture: " + edge.ToString() + "  Edge != null");
            if (edge.To != null)
            {
                //Debug.Log(this.id + "  AddDeparture: " + edge.ToString() + "  Edge.To != null");


                // This bit me in the ass...  works perfect for the edge-creation, but NOT for Merging Nodes
                //if (!this.Departures.Contains(edge)
                //     && this.Graph.GetEdge(edge.From, edge.To) == null
                //     && this.Graph.GetEdge(edge.To, edge.From) == null)
                //{

                if (!this.Departures.Contains(edge))
                {
                    //Debug.Log(this.id + "  AddDeparture: " + edge.ToString() + "  ReallyAddedDeparture");
                    this.Departures.Add(edge);
                    edge.Changed += OnEdgeEvent;
                    if (fireEvent)
                        OnNodeChanged();
                    //Debug.Log(this.id + "  AddDeparture: " + edge.ToString() + "  AfterAddingDeparture\n" + this.ToStringExtended());
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public bool RemoveDeparture(Edge edge)
    {
        Debug.Log(this.id + "  RemoveDeparture: " + edge.ToString());
        return RemoveDeparture(edge, true);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="fireEvent"></param>
    /// <returns></returns>
    public bool RemoveDeparture(Edge edge, bool fireEvent)
    {
        edge.Changed -= OnEdgeEvent;
        bool success = Departures.Remove(edge);
        if (success)
        {
            RefreshEdges();
            if (fireEvent)
                OnNodeChanged();
        }

        return success;
    }



    /// <summary>
    /// 
    /// </summary>
    public void ClearDepartures()
    {
        if (this.Departures.Count == 0)
            return;

        List<Edge> concurrentDepartures = new List<Edge>(Departures);

        foreach (Edge deleteEdge in concurrentDepartures)
        {
            Departures.Remove(deleteEdge);
            RefreshEdges();
            deleteEdge.Delete();
        }

        this.departures = new List<Edge>();
        RefreshEdges();
    }
    #endregion // Departure-Methods

    #region Combined Methods
    public bool RemoveEdge(Edge edge)
    {
        bool found = false;
        if (Arrivals.Contains(edge))
            found = RemoveArrival(edge);
        else if (Departures.Contains(edge))
            found = RemoveDeparture(edge);

        return found;
    }

    public void ClearEdges()
    {
        ClearArrivals();
        ClearDepartures();
        this.edges = new List<Edge>();
    }
    #endregion // Combined Methods
    #endregion // Arrival- and -Departure-Methods

    #region Edge-related Helpers
    /// <summary>
    /// 
    /// </summary>
    /// <param name="edges"></param>
    /// <param name="NodeBehaviour"></param>
    /// <returns></returns>
    public List<Edge> SortEdges(List<Edge> edges, Node node)
    {
        // if there is only one edge, the list is sorted

        if (edges.Count < 2)
            return edges;

        float[] angles = new float[edges.Count];

        for (int i = 0; i < edges.Count; i++)
        {
            Edge edge = edges[i];

            if (node == edge.From)
                angles[i] = edge.AngleToZAxisFrom;
            else if (node == edge.To)
                angles[i] = edge.AngleToZAxisTo;
        }

        Edge[] edgesArray = edges.ToArray<Edge>();
        Array.Sort(angles, edgesArray);
        //        for (int i = 0; i < edgesArray.Length; i++)
        //            Debug.Log("angles["+i+"] = " + angles[i]);

        return edgesArray.ToList<Edge>();
    }


    /// <summary>
    /// Sort the edges-list clockwise according using their angle to the z-axis 
    /// </summary>
    public void SortEdges()
    {
        this.edges = SortEdges(this.edges, this);
    }


    public void AssignEdgeValues()
    {
        //Debug.Log("AssignEdgeValues on node: " + this.id);

        foreach (Edge edge in this.arrivals)
            edge.RefreshAdjacentEdgeReferencesTo();

        foreach (Edge edge in this.departures)
            edge.RefreshAdjacentEdgeReferencesFrom();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float MinEdgeLength()
    {
        float length = float.MaxValue;
        foreach (Edge edge in arrivals)
        {
            if (edge.Vector.magnitude < length)
                length = edge.Vector.magnitude;
        }

        foreach (Edge edge in departures)
        {
            if (edge.Vector.magnitude < length)
                length = edge.Vector.magnitude;
        }

        return length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float MaxEdgeLength()
    {
        float length = float.MinValue;
        foreach (Edge edge in arrivals)
        {
            if (edge.Vector.magnitude > length)
                length = edge.Vector.magnitude;
        }

        foreach (Edge edge in departures)
        {
            if (edge.Vector.magnitude > length)
                length = edge.Vector.magnitude;
        }

        return length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public Edge NextEdgeFor(Edge edge)
    {
        List<Edge> edges = this.Edges;
        int indexOfEdge = edges.IndexOf(edge);

        if (indexOfEdge >= 0)
        {
            if (indexOfEdge == this.edges.Count - 1)
                return this.edges[0];
            else
                return this.edges[indexOfEdge + 1];
        }
        return null;
    }



    public string printEdges()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(this.id);
        for (int i = 0; i < this.edges.Count; i++)
        {
            sb.Append(i).Append(" ").Append(this.edges[i].Id).Append('\n');
        }
        return sb.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public float NextEdgeAngleFor(Edge edge)
    {
        int indexOfEdge = this.edges.IndexOf(edge);

        Edge nextEdge = NextEdgeFor(edge);

        if (indexOfEdge == this.edges.Count - 1)
            return 360f - MathUtils.AngleToZAxis(edge, this) + MathUtils.AngleToZAxis(nextEdge, this);
        else if (indexOfEdge >= 0)
            return MathUtils.AngleToZAxis(nextEdge, this) - MathUtils.AngleToZAxis(edge, this);

        return 0f;
    }


    /// <summary>
    /// Returns the previous edge (first counter-clockwise on XZ-Plane)
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public Edge PreviousEdgeFor(Edge edge)
    {
        List<Edge> edges = this.Edges;
        int indexOfEdge = edges.IndexOf(edge);

        //Debug.Log(this.id + ": " + edges.Count + " edges --> index of edge("+ edge.Id + ") = " + indexOfEdge);

        if (indexOfEdge >= 0)
        {
            if (indexOfEdge == 0)
                return this.edges[this.edges.Count - 1];
            else
                return this.edges[indexOfEdge - 1];
        }
        return null;
    }


    /// <summary>
    /// Returns the angle between the given edge and its previous edge (clockwise on XZ-Plane) 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns>Angle between edge and its previous edge</returns>
    public float PreviousEdgeAngleFor(Edge edge)
    {
        int indexOfEdge = this.edges.IndexOf(edge);
        Edge previousEdge = PreviousEdgeFor(edge);

        //if (previousEdge != null)
        //{
        if (indexOfEdge == 0)
            return 360f - MathUtils.AngleToZAxis(previousEdge, this) + MathUtils.AngleToZAxis(edge, this);
        else if (indexOfEdge > 0)
            return MathUtils.AngleToZAxis(edge, this) - MathUtils.AngleToZAxis(previousEdge, this);
        //}
        return 0f;
    }
    #endregion

    #region Node-related Helpers
    /// <summary>
    /// Merges Nodes and deletes Edges between the merged Nodes
    /// </summary>
    /// <param name="nodes"></param>
    public void MergeWith(params Node[] nodes)
    {
        //Debug.Log(this.id + " MergeNodes ");

        foreach (Node node in nodes)
        {
            Edge connectingEdge;
            if (this.IsConnectedTo(node, out connectingEdge))
            {
                if (this.Degree == 1)
                {
                    if (nodes[0].Degree == 1)
                    {
                        connectingEdge.Delete();
                        return;
                    }
                    else
                    {
                        Node tmp = nodes[0];
                        nodes[0] = this;
                        tmp.MergeWith(nodes);
                        return;
                    }
                }
                
                connectingEdge.Delete();
            }
            
            List<Edge> concurrentEdges = new List<Edge>(node.edges);
            foreach (Edge edge in concurrentEdges)
            {
                if (this.IsConnectedTo(edge.From) || this.IsConnectedTo(edge.To))
                    edge.Delete();
                else
                {
                    if (edge.From == node)
                        edge.From = this;
                    else if (edge.To == node)
                        edge.To = this;
                }
            }
                        
            node.Delete();
            this.OnNodeChanged();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="connectingEdge"></param>
    /// <returns></returns>
    public bool IsConnectedTo(Node otherNode, out Edge connectingEdge)
    {
        connectingEdge = null;

        foreach (Edge edge in departures)
        {
            if (edge.To == otherNode)
            {
                connectingEdge = edge;
                return true;
            }
        }

        foreach (Edge edge in arrivals)
        {
            if (edge.From == otherNode)
            {
                connectingEdge = edge;
                return true;
            }
        }

        return false;
    }



    public bool IsConnectedTo(Node otherNode)
    {
        foreach (Edge edge in departures)
        {
            if (edge.To == otherNode)
                return true;
        }

        foreach (Edge edge in arrivals)
        {
            if (edge.From == otherNode)
                return true;
        }
        return false;
    }

    #endregion // Node-related Helpers
    #endregion // Methods

    #region Observer
    #region Observer Handling
    [SerializeField]
    private event EventHandler _changeGraphLayer = delegate { };
    public event EventHandler ChangedGraphLayer
    {
        add { _changeGraphLayer += value; }
        remove { _changeGraphLayer -= value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public void RefreshEdges()
    {
        this.edges = GetAllEdges(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnNodeChanged()
    {
        //Debug.Log(" Before +++++++++++  " + this.id + "OnNodeChanged()");
        RefreshEdges();
        _changeGraphLayer(this, new GraphEventArgs(this, null, GraphEventType.NodeChanged, 3, this.id + ": NodeChanged"));
        //Debug.Log(" After  +++++++++++  " + this.id + "OnNodeChanged()");
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnNodeDeleted()
    {
        _changeGraphLayer(this, new GraphEventArgs(this, null, GraphEventType.NodeDeleted, 3, this.id + ": NodeDeleted"));
    }


    //public void OnEdgesChanged()
    //{
    //    this.edges = GetAllEdges(true);
    //    //Debug.Log("Refreshed Edges on node: " + this.id);
    //}

    #endregion // Observer Handling

    #region Observe Others
    private EventHandler onEdgeEvent;
    private void OnEdgeEvent(System.Object sender, EventArgs e)
    {
        GraphEventArgs graphEventArgs = (GraphEventArgs)e;

        // Catch PingPong
        if (graphEventArgs.LastSender == this)
            return;

        if (graphEventArgs.TimeToLive > 0)
        {
            GraphEventArgs args;

            // Just to make sure
            RefreshEdges();

            switch (graphEventArgs.GraphEvent)
            {
                case GraphEventType.NodeChanged:
                    //Debug.Log(this.id + "NodeChanged\n" + graphEventArgs.ToString());
                    args = graphEventArgs.nextLevelArgs(sender, GraphEventType.NodeChanged, this.id + ".NodeChanged");
                    _changeGraphLayer(this, args);
                    break;
                case GraphEventType.NodeDeleted:
                    //Debug.Log(this.id + ": NodeDeleted\n" + graphEventArgs.ToString());
                    break;
                case GraphEventType.EdgeFromChanged:
                    //Debug.Log(this.id + ": EdgeFromChanged\n" + graphEventArgs.ToString());
                    args = graphEventArgs.nextLevelArgs(sender, GraphEventType.NodeChanged, this.id + ".NodeChanged");
                    _changeGraphLayer(this, args);
                    break;
                case GraphEventType.EdgeToChanged:
                    //Debug.Log(this.id + ": EdgeToChanged\n" + graphEventArgs.ToString());
                    args = graphEventArgs.nextLevelArgs(sender, GraphEventType.NodeChanged, this.id + ".NodeChanged");
                    _changeGraphLayer(this, args);
                    break;
                case GraphEventType.EdgeDeleted:
                    //Debug.Log(this.id + ": EdgeDeletedChanged\n" + graphEventArgs.ToString());
                    args = graphEventArgs.nextLevelArgs(sender, GraphEventType.EdgeDeleted, this.id + ".EdgeDeleted");
                    _changeGraphLayer(this, args);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion // Observe others

    public List<string> PrintEventHandlers()
    {
        List<string> names = new List<string>();
        foreach (Delegate del in _changeGraphLayer.GetInvocationList())
            names.Add(del.Target + ": " + del.Method.Name);
        return names;
    }
    #endregion // Observer


    public string ToStringExtended()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Node(" + this.id + ", " + Position.ToString() + ")");
        sb.AppendLine();
        sb.AppendLine("Arrivals:");
        foreach (Edge edge in this.arrivals)
        {
            sb.AppendLine(edge.ToString());
        }
        sb.AppendLine();
        sb.AppendLine("Departures:");
        foreach (Edge edge in this.departures)
        {
            sb.AppendLine(edge.ToString());
        }
        sb.AppendLine();
        sb.AppendLine("Edges:");
        foreach (Edge edge in this.edges)
        {
            sb.AppendLine(edge.ToString());
        }
        return sb.ToString();
    }


    public override string ToString()
    {
        return "Node(" + this.id + ", " + Position.ToString() + ")";
    }

    #region IHasID Member
    public string GetID()
    {
        return this.id;
    }
    #endregion // IHasID

}

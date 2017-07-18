using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class Edge : IHasID
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

    public static bool fireEvents = true;

    // The start-node of the Edge
    [SerializeField]
    private Node from;
    public Node From
    {
        get { return from; }
        set
        {
            if ((value != null) && (this.to != null) && (this.to != value))
            {
                // Detach from previously set start node
                if (this.from != null)
                    this.from.RemoveDeparture(this);
                this.from.ChangedGraphLayer -= onNodeEvent;

                // Set and attach to new start-node
                this.from = value;
                this.from.ChangedGraphLayer += onNodeEvent;
                this.from.AddDeparture(this);

                // Recalculate the edges values
                RefreshEdge();

                if (fireEvents)
                {
                    // Inform Listeners
                    OnFromNodeChanged();
                }
            }
            else
            {
                //if (this.to == value)
                //{
                //    this.from = value;
                //    Delete();
                //}

                //throw new UnityException("Property From: Value could not be set because at least one was null! (Edge.From)");
            }
        }
    }
   

    // The end-node of the edge
    [SerializeField]
    private Node to;
    public Node To
    {
        get { return to; }
        set
        {
            if ((value != null) && (this.from != null) && (this.from != value))
            {
                // Detach from previously set end node
                Debug.Log(this.Id + ": setTO --> " + value);
                if (this.to != null)
                    this.to.RemoveArrival(this);
                this.to.ChangedGraphLayer -= onNodeEvent;
                
                // Set and attach to new end-node
                this.to = value;
                this.to.ChangedGraphLayer += onNodeEvent;
                this.to.AddArrival(this);

                // Recalculate the edges values
                //Debug.Log(this.Id + ": Before RefreshEdge");
                RefreshEdge();
                //Debug.Log(this.Id + ": After RefreshEdge");

                if (fireEvents)
                {
                    // Inform Listeners
                    //Debug.Log(this.Id + ": Before OnToNodeChanged");
                    OnToNodeChanged();
                    //Debug.Log(this.Id + ": After OnToNodeChanged");
                }
            }
            else
            {
                //if (this.from == value)
                //{
                //    this.to = value;
                //    Delete();
                //}
                //throw new UnityException("Property To: Value could not be set because at least one was null! (Edge.To)");
            }
        }
    }
    #endregion // Properties (Schema)

    #region Derived Properties
    [SerializeField]
    private Vector3 position;
    public Vector3 Position
    {
        get { return position; }
        set
        {
            Vector3 oldPosition = position;
            position = value;

            // calculate position-difference
            Vector3 difference = position - oldPosition;

            // apply position-change to NodeBehaviours
            From._position += difference;
            To._position += difference;

            // Recalculate the edges values
            RefreshEdge();

            // Inform Listeners
            OnPositionChanged();
        }
    }

   

    [SerializeField]
    private Vector3 vector;
    public Vector3 Vector
    {
        get { return this.vector; }
    }

    [SerializeField]
    private Vector3 vectorFrom;
    public Vector3 VectorFrom
    {
        get { return this.vectorFrom; }
    }

    [SerializeField]
    private Vector3 vectorTo;
    public Vector3 VectorTo
    {
        get { return this.vectorTo; }
    }

    [SerializeField]
    private Vector3 vectorNormalized;
    public Vector3 VectorNormalized
    {
        get { return this.vectorNormalized; }
    }

    [SerializeField]
    private float length;
    public float Length
    {
        get { return length; }
    }

    [SerializeField]
    private Vector3 orthoNormalVector;
    public Vector3 OrthoNormalVector
    {
        get { return orthoNormalVector; }
    }

    [SerializeField]
    private Vector3 orthoNormalVectorUp;
    public Vector3 OrthoNormalVectorUp
    {
        get { return orthoNormalVectorUp; }
    }

    [SerializeField]
    private float angleToZAxisFrom;
    public float AngleToZAxisFrom
    {
        get { return angleToZAxisFrom; }
    }

    [SerializeField]
    private float angleToZAxisTo;
    public float AngleToZAxisTo
    {
        get { return angleToZAxisTo; }
    }

    private Edge nextEdgeFrom;
    public Edge NextEdgeFrom
    {
        get { return nextEdgeFrom; }
        set { nextEdgeFrom = value; }
    }

    private float nextEdgeFromAngle;
    public float NextEdgeFromAngle
    {
        get { return nextEdgeFromAngle; }
        set { nextEdgeFromAngle = value; }
    }

    private Edge nextEdgeTo;
    public Edge NextEdgeTo
    {
        get { return nextEdgeTo; }
        set { nextEdgeTo = value; }
    }

    private float nextEdgeToAngle;
    public float NextEdgeToAngle
    {
        get { return nextEdgeToAngle; }
        set { nextEdgeToAngle = value; }
    }

    private Edge previousEdgeFrom;
    public Edge PreviousEdgeFrom
    {
        get { return previousEdgeFrom; }
        set { previousEdgeFrom = value; }
    }

    private float previousEdgeFromAngle;
    public float PreviousEdgeFromAngle
    {
        get { return previousEdgeFromAngle; }
        set { previousEdgeFromAngle = value; }
    }

    private Edge previousEdgeTo;
    public Edge PreviousEdgeTo
    {
        get { return previousEdgeTo; }
        set { previousEdgeTo = value; }
    }

    private float previousEdgeToAngle;
    public float PreviousEdgeToAngle
    {
        get { return previousEdgeToAngle; }
        set { previousEdgeToAngle = value; }
    }
    #endregion // Derived Properties
    #endregion // Properties

    #region Constructors
    public Edge()
    {

    }

   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public Edge(Graph graph, string id, Node from, Node to)
    {
        // Assign the parent-object
        this.graph = graph;

        // Instantiate a new Eventhandler to Handle Graph-Events from the nodes
        onNodeEvent = new EventHandler(OnNodeEvent);

        // Set the edges ID
        this.id = id;

        // Set the from- and to-nodes (and hope they aren't null...)
        //Debug.Log("Edge-Constructor: SetFromTo(" + from.Id + ", " + to.Id + ")");
        SetFromTo(from, to);
    }
    #endregion // Constructors

    #region Destructor
    /// <summary>
    /// Deletes the Edge
    /// All listeners will be detached
    /// </summary>
    /// <returns></returns>
    public Edge Delete()
    {
        // If the self-deletion has not bee done yet
        if ((this.From != null) && (this.To != null))
        {
            // Fire a Graph-Event to inform others that the edge has been deleted
            OnDelete();
            //Debug.Log("-------------------------------- Edge Delete : nach dem feuern");

            this.from.RemoveDeparture(this, false);
            this.to.RemoveArrival(this, false);
            //Debug.Log("After removing Arrival and Departure");

            // Detach the EventHandler from the nodes (stop listening)
            this.from.ChangedGraphLayer -= onNodeEvent;
            this.to.ChangedGraphLayer -= onNodeEvent;

            Node oldFromNode = this.From;
            Node oldToNode = this.To;

            //Debug.Log("fromDegree = " + oldFromNode.Degree + "  toDegree = " + oldToNode.Degree);

            // Set the endings NULL
            this.from = null;
            this.to = null;
            //Debug.Log("After setting From and To NULL");

            // Remove self from the graph
            this.graph.RemoveEdge(this);
            //Debug.Log("After removing Edge from Graph");

            if (oldFromNode.Degree == 0)
            {
                //Debug.Log("Delete oldFromNode");
                oldFromNode.Delete();
            }
            else
                oldFromNode.OnNodeChanged();
            //Debug.Log("After oldFrom.OnNodeChanged()");


            if (oldToNode.Degree == 0)
            {
                //Debug.Log("Delete oldToNode");
                oldToNode.Delete();
            }
            else
                oldToNode.OnNodeChanged();
            //Debug.Log("After oldTo.OnNodeChanged()");
           
        }
        return this;
    }
    #endregion // Destructor

    #region Methods
    #region Ending-related Methods
    /// <summary>
    /// Sets the end-nodes of the edge - Use this one after an Edge was created
    /// Setting the ends separately after creation of the edge will FAIL
    /// If the ends where already set, the old listeners will be detached and the new ones will be attached
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void SetFromTo(Node from, Node to)
    {
        // Make sure none of the new nodes is NULL
        // and that they aren't the same node
        if ((from != null) && (to != null) && from != to)
        {
            // If the edge already had a from-node...
            if (this.from != null && this.from != from)
            {
                // ... detach from it by removing from the departures-list..
                this.from.RemoveDeparture(this);
                // ... and by removing the EventHandler (stop listening)
                this.from.ChangedGraphLayer -= onNodeEvent;
            }

            // Same as before: If the edge already had a to-node...
            if (this.to != null && this.to != to)
            {
                // ... detach from it by removing from the arrivals-list..
                this.to.RemoveArrival(this);
                // ... and by removing the EventHandler (stop listening)
                this.to.ChangedGraphLayer -= onNodeEvent;
            }

            // Assign the new from-node...
            this.from = from;
            // ... and start listening to its Events
            from.ChangedGraphLayer += onNodeEvent;

            // Assign the new to-node...
            this.to = to;
            // ... and start listening to its Events
            to.ChangedGraphLayer += onNodeEvent;
            //Debug.Log("Set From + To Node on Edge " + this.id);

            // Attach to the nodes by adding the edge to their adjacency-lists 
            this.from.AddDeparture(this, false);
            this.to.AddArrival(this, false);

            //Debug.Log("Added Edge " + this.id + " as Departing on " + this.from.Id + " and arriving Edge on " + this.to.Id);

            // Recalculate values since the endings and direction (may) have changed
            RefreshEdge();
            //Debug.Log("Refreshed Edge: " + this.id);

            //Debug.Log(" ------< Before OnFromToChanged " + this.id + " >--------");
            OnFromToChanged();
            //Debug.Log(" ------< After OnFromToChanged  " + this.id + " >--------");
        }
        else
        {
            // If both nodes had been set, leave them as is...
            if (this.from != null && this.to != null)
            {
                throw new UnityException("Edges from- and to-Attribute could not be set because at least one was null! (Edge.SetFromTo) Leaving the nodes as is.");
            }
            else // ... otherwise if one is null, detach from the other node.
            {
                if (this.from != null)
                {
                    this.from.RemoveDeparture(this);
                    this.from.ChangedGraphLayer -= onNodeEvent;
                    this.from = null;
                }

                if (this.to != null)
                {
                    this.to.RemoveArrival(this);
                    this.to.ChangedGraphLayer -= onNodeEvent;
                    this.to = null;
                }
                //throw new UnityException("Edges from- and to-Attribute could not be set because at least one was null! (Edge.SetFromTo) Setting both to null.");
                this.Delete();
            }
        }
    }

  

    /// <summary>
    /// Switch the adjacent Nodes
    /// </summary>
    public void SwitchEnds()
    {
        // Switch the Ends
        SetFromTo(this.to, this.from);
        OnEndsSwitched();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Node OtherEnd(Node node)
    {
        if (node == this.from)
            return this.to;

        if (node == this.to)
            return this.from;

        return null;
    }
    #endregion Ending-related Methods


    public bool IsConnectedTo(Node node)
    {
        bool result = false;

        if (this.from == node || this.to == node)
            return true;

        return result;
    }

    public bool IsConnectedTo(Edge edge)
    {
        bool result = false;

        if (this.from.Edges.Contains(edge))
            return true;

        if (this.to.Edges.Contains(edge))
            return true;

        return result;
    }

    public bool IsConnectedTo(Edge edge, out List<Edge> otherEdges)
    {
        bool result = false;
        otherEdges = new List<Edge>();

        foreach (Edge fromEdge in this.from.Edges)
        {
            if (fromEdge.IsConnectedTo(edge))
                otherEdges.Add(fromEdge);
        }

        foreach (Edge toEdge in this.to.Edges)
        {
            if (toEdge.IsConnectedTo(edge))
                otherEdges.Add(toEdge);
        }

        return result;
    }



    #region Value Calculation
    /// <summary>
    /// Recalculate the edges vectors (direction, orhtogonalnNormalized, orthogonalNormalizedUp), 
    /// the angle to the z-axis for both from- and to-node and the midpoint, as well as the length of the edge
    /// 
    /// </summary>
    public void RefreshEdge()
    {
        if ((from != null) && (to != null))
        {
            // Calculate vectors
            this.vector = this.To.Position - this.From.Position;
            this.vectorFrom = this.vector;
            this.vectorTo = -this.vector;
            this.vectorNormalized = this.vector.normalized;
            this.orthoNormalVector = MathUtils.GetOrthogonalNormalizedVector(this);
            this.orthoNormalVectorUp = MathUtils.GetOrthogonalNormalizedVectorUp(this);
            
            // Calculate Angles
            this.angleToZAxisFrom = MathUtils.AngleToZAxis(this, this.From);
            this.angleToZAxisTo = MathUtils.AngleToZAxis(this, this.To);

            // Calculate MidPoint
            this.position = GetMidPoint();

            // Calculate the length/magnitude of the edge 
            this.length = this.vector.magnitude;
        }
    }
    
    
    /// <summary>
    /// Get the references to the adjacent (previous and next) edges
    /// and calculate the angles between them
    /// The edges-list of the nodes have to be sorted.
    /// </summary>
    public void RefreshAdjacentEdgeReferences()
    {
        RefreshAdjacentEdgeReferencesFrom();
        RefreshAdjacentEdgeReferencesTo();
    }


    public void RefreshAdjacentEdgeReferencesOnNode(Node node)
    {
        if (node == this.from)
            RefreshAdjacentEdgeReferencesFrom();
        else if (node == this.to)
            RefreshAdjacentEdgeReferencesTo();
    }

    public void RefreshAdjacentEdgeReferencesTo()
    {
        //Debug.Log(this.id + ": Refresh TO");
        this.nextEdgeTo = this.to.NextEdgeFor(this);
        this.previousEdgeTo = this.to.PreviousEdgeFor(this);

        this.nextEdgeToAngle = this.to.NextEdgeAngleFor(this);
        this.previousEdgeToAngle = this.to.PreviousEdgeAngleFor(this);
    }

    public void RefreshAdjacentEdgeReferencesFrom()
    {
        //Debug.Log(this.id + ": Refresh FROM");
        this.nextEdgeFrom = this.from.NextEdgeFor(this);
        this.previousEdgeFrom = this.from.PreviousEdgeFor(this);

        this.nextEdgeFromAngle = this.from.NextEdgeAngleFor(this);
        this.previousEdgeFromAngle = this.from.PreviousEdgeAngleFor(this);
    }



    /// <summary>
    /// Calculate the midpoint/center of the edge
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMidPoint()
    {
        return this.From.Position + (this.To.Position - this.From.Position) * 0.5f;
    }

    /// <summary>
    /// Get the look-direction to the other node from the given nodes point of view
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Vector3 GetLookDirectionFromNode(Node node)
    {
        if (node == this.From)
            return this.To.Position - this.From.Position;
        else
            return this.From.Position - this.To.Position;
    }
    #endregion // Value Calculation
    #endregion // Methods

    #region Observer
    #region Observer Handling
    [SerializeField]
    private event EventHandler _change = delegate { };
    public event EventHandler Changed
    {
        add { _change += value; }
        remove { _change -= value; }
    }
    
    /// <summary>
    /// Fire an event that the edges from-node changed
    /// </summary>
    public void OnFromNodeChanged()
    {
        From.OnNodeChanged();
        _change(this, new GraphEventArgs(this, null, GraphEventType.EdgeFromChanged, 3, this.id + ": EdgeFromChanged"));
    }

    /// <summary>
    /// Fire an event that the edges to-node changed
    /// </summary>
    public void OnToNodeChanged()
    {
        To.OnNodeChanged();
        _change(this, new GraphEventArgs(this, null, GraphEventType.EdgeToChanged, 3, this.id + ": EdgeToChanged"));
    }


    /// <summary>
    /// Fire an event that the edges from- and to node have been switched (to, from)
    /// </summary>
    public void OnEndsSwitched()
    {
        _change(this, new GraphEventArgs(this, null, GraphEventType.EdgeEndsSwitched, 3, this.id + ": EdgeEndsSwitched"));
    }


    /// <summary>
    /// Fire an event that the edges from- and to node changed
    /// </summary>
    public void OnFromToChanged()
    {
        //Debug.Log(this.id + " Edge: OnFromToChanged  -- > OnNodeChanged for both from and to Node:");    
        this.from.OnNodeChanged();
        this.to.OnNodeChanged();
    }

    /// <summary>
    /// Fire an event that the edges position changed. Includes fireing the NodePositionChanged-event on the from- and to node.
    /// </summary>
    private void OnPositionChanged()
    {
        this.from.OnNodeChanged();
        this.to.OnNodeChanged();
        //_change(this, new GraphEventArgs(null, GraphEventType.EdgeDeleted, 3, this.id + ": EdgeDeleted"));
    }

    /// <summary>
    /// Fire an event that the edge has been deleted
    /// </summary>
    private void OnDelete()
    {
        //Debug.Log(this.id + ": Edge.OnDelete()");
        _change(this, new GraphEventArgs(this, null, GraphEventType.EdgeDeleted, 3, this.id + ": EdgeDeleted"));
        //Debug.Log(this.id + ": After Edge.OnDelete()");
    }
    #endregion // Observer Handling

    #region Observe Others
    private EventHandler onNodeEvent;
    private void OnNodeEvent(System.Object sender, EventArgs e)
    {
        GraphEventArgs graphEvent = (GraphEventArgs)e;
        //Debug.Log(this.id + ": " + graphEvent.ToString());

        //Debug.Log(sender.ToString() + " --> " + this.id + ": received --> " + graphEvent.ToString());

        if (graphEvent.LastSender == (Edge) this)
        {
            //Debug.Log("Edge PingPong-BREAK! " + this.id);
            return;
        }

        GraphEventArgs newGraphEventArgs;
        // Filter PingPong
        if (graphEvent.TimeToLive > 0)
        {
            switch (graphEvent.GraphEvent)
            {
                case GraphEventType.NodeChanged:
                    // Recalculate Values
                    RefreshEdge();

                    if ((Node)sender == this.from)
                    {
                        //Debug.Log(this.id + " Edge: NodeChanged (sender" + ((Node)sender).ToString() + ")  --> fire EdgeFromChanged");    
                        newGraphEventArgs = graphEvent.nextLevelArgs(sender, GraphEventType.EdgeFromChanged, this.id + ".FromChanged");
                    }
                    else
                    {
                        //Debug.Log(this.id + " Edge: NodeChanged (sender" + ((Node)sender).ToString() + ")  --> fire EdgeToChanged");    
                        newGraphEventArgs = graphEvent.nextLevelArgs(sender, GraphEventType.EdgeToChanged, this.id + ".ToChanged");
                    }

                    _change(this, newGraphEventArgs);
                    break;
                case GraphEventType.NodeDeleted:
                    //Debug.Log(this.id + " Edge: NodeDeleted (sender" + ((Node)sender).ToString() + ")  --> Delete()  -- > fire EdgeDeleted");    
                    Delete();
                    _change(this, graphEvent.nextLevelArgs(sender, GraphEventType.EdgeDeleted, this.id + ".EdgeDeleted"));
                    break;
                case GraphEventType.EdgeChanged:
                    //Debug.Log(this.id + " Edge: EdgeChanged (sender" + ((Node)sender).ToString() + ")  --> Do Nothing");    
                    //_change(this, graphEvent.nextLevelArgs(sender, GraphEventType.EdgeDeleted, this.id + ".EdgeDeleted"));
                    break;
                default:
                    //Debug.Log("Default???");
                    break;
            }
        }
    }
    #endregion // Observe Others
    #endregion // Observer

    public override string ToString()
    {
        if (from == null || to == null)
            return "Edge(" + this.id + ")";

        return "Edge(" + this.id + "," + from.ToString() + ", " + to.ToString() + ")";
    }

    #region IHasID Member
    public string GetID()
    {
        return this.id;
    }
    #endregion

}

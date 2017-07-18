using System.Collections.Generic;
using System.Text;
using UnityEngine;


/// <summary>
/// The Graph
/// 
/// TODO: Class-Comment
/// 
/// </summary>
public class Graph
{
    #region Properties
    #region Properties (Schema)
    [SerializeField]
    private List<Node> nodes;
    public List<Node> Nodes
    {
        get { return nodes; }
    }

    [SerializeField]
    private List<Edge> edges;
    public List<Edge> Edges
    {
        get { return edges; }
    }
    #endregion // Properties (Schema)
    #endregion // Properties

    #region Constructors
    /// <summary>
    ///  DefaultConstructor
    /// </summary>
    public Graph()
    {
        // Instantiate new node- and edges-lists
        this.nodes = new List<Node>();
        this.edges = new List<Edge>();
    }
    #endregion // Constructors


    #region Methods
    #region Node-Methods
    /// <summary>
    /// Returns a Node with the given ID (if exits).
    /// </summary>
    /// <param name="id">The ID of the desired node</param>
    /// <returns>A Node with the given ID (if exists), NULL if none available.</returns>
    public Node GetNodeByID(string id)
    {
        // Iterate over all edges to find...
        foreach (Node node in this.nodes)
            // ... the node with the desired ID
            if (node.Id == id)
                return node;

        // If the node has not been found, return NULL
        return null;
    }
    
    /// <summary>
    /// Creates a new Node with the given position, generating a unique ID
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Node CreateNode(Vector3 position)
    {
        //Debug.Log("Graph: Create Node at Position " + position);
        // Declare and instantiate a new node with a unique ID...
        Node newNode = new Node(
            this, 
            IDUtils.GetUniqueID("Node", string.Empty, this.nodes, 10000), 
            position);

        // ... add the node to the nodes-list...
        AddNode(newNode);

        // ... and return the new node.
        return newNode;
    }

    /// <summary>
    /// Adds the given node to the nodes-list.
    /// </summary>
    /// <param name="node">The node that should be added to the nodes-list.</param>
    public void AddNode(Node node)
    {
        // If not already in the nodes-list, add the given node
        if (!this.nodes.Contains(node))
            this.nodes.Add(node);
    }

    /// <summary>
    /// Simply removes the node from the graph (Also deletes the node).
    /// </summary>
    /// <param name="node">The node that should be removed from the nodes-list.</param>
    /// <returns></returns>
    public bool RemoveNode(Node node, bool deleteNode)
    {
        // Remove the given node from the nodes-list after deleting it (removing adjacent edges)
        if(deleteNode)
            return this.nodes.Remove(node.Delete());
        else
            return this.nodes.Remove(node);
    }
    #endregion // Node-Methods
        
    #region Edge-Methods
    /// <summary>
    /// Returns an edge with the given ID (if exists)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns an edge with the given ID (if exists), NULL if none available.</returns>
    public Edge GetEdgeByID(string id)
    {
        // Iterate over all edges to find...
        foreach (Edge edge in this.edges)
            // ... the edge with the desired ID
            if (edge.Id == id)
                return edge;
        
        // If the edge has not been found, return NULL
        return null;
    }

    /// <summary>
    /// Adds the given edge to the edges-list.
    /// If from- or to-node are not yet available in the nodes-list, they will be added as well.
    /// </summary>
    /// <param name="edge">The edge that should be added to the edges-list.</param>
    public void AddEdge(Edge edge)
    {
        // Add the Edge to the edges List
        this.edges.Add(edge);
        
        // If not yet part of the nodes-list, add the From-Node
        if (!this.nodes.Contains(edge.From))
            AddNode(edge.From);

        // If not yet part of the nodes-list, add the From-Node
        if (!this.nodes.Contains(edge.To))
            AddNode(edge.To);
    }

    /// <summary>
    /// Removes the given edge from the edges-list.
    /// If the from- and to-node are allready null, signs are that the edge has been deleting itself.
    /// Otherwise, the edge should first be deleted (setting the from- and to-node to NULL) and afterwards be removed from the edges-list.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns>TRUE if the removal has been successfull, FALSE if the edge did not exist in the list.</returns>
    public bool RemoveEdge(Edge edge)
    {
        // if the edge already deleted itself...  sounds weird... ;o)
        if(edge.From == null && edge.To == null)
            return this.edges.Remove(edge); // just remove it...
        
        // ... otherwise tell it to kill itself... sounds even weirder...
        return this.edges.Remove(edge.Delete());
    }

    /// <summary>
    /// Creates a new Edge between the given from- and to-node and adds it to the edges-list
    /// </summary>
    /// <param name="from">The start-node of the new edge</param>
    /// <param name="to">The end-node of the new edge</param>
    /// <returns>The newly created edge.</returns>
    public Edge CreateEdge(Node from, Node to)
    {
        //Debug.Log("Graph: Create Edge for From-Node " + from.Id + " and To-Node " + to.Id);
        // Declare and instantiate a new Edge between the given nodes with a unique ID...
        Edge newEdge = new Edge(
            this,
            IDUtils.GetUniqueID("Edge", string.Empty, this.edges, 10000),
            from, to);
        
        // ... add the new edge to the edges-list...
        AddEdge(newEdge);

        // ...and return the new edge.
        return newEdge;
    }

	/// <summary>
	/// Returns an Edge between the two Nodes (if exists) regarding the from-to-direction
	/// </summary>
	/// <param name="from">The start-node of the desired edge</param>
    /// <param name="to">The end-node of the desired edge</param>
	/// <returns>An edge (if exists) with 'from' as start-node and 'to' as end-node. Returns NULL if none available.</returns>
	public Edge GetEdge(Node from, Node to) 
	{
        // catch invalid parameters
        if (from == null || to == null)
            return null;

        // Iterate through the edges...
		foreach (Edge edge in this.edges) {
            // ... until an edge has the desired from- and -to.node
			if (edge.From == from && edge.To == to)
				return edge;
		}

        // If no matching edge was found, return NULL
		return null;
	}
    #endregion // Edge-Methods



    //public void FindIntersections


    #endregion //Methods

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Graph:");
        sb.Append("Nodes (").Append(nodes.Count).Append("):");

        foreach (Node node in nodes)
            sb.AppendLine(node.ToString());
        
        sb.AppendLine();
        sb.Append("Edges (").Append(edges.Count).Append("):");
        foreach (Edge edge in edges)
            sb.AppendLine(edge.ToString());

        return sb.ToString();
    }

}



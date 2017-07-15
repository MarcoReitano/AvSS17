using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Graph_Behaviour : MonoBehaviour
{
    [SerializeField]
    public Graph graph = new Graph();
    public Graph Graph
    {
        get { return graph; }
        set { graph = value; }
    }

    [SerializeField]
    private List<Node_Behaviour> nodeBehaviours = new List<Node_Behaviour>();
    public List<Node_Behaviour> NodeBehaviours
    {
        get { return nodeBehaviours; }
        set { nodeBehaviours = value; }
    }

    [SerializeField]
    private List<Edge_Behaviour> edgeBehaviours = new List<Edge_Behaviour>();
    public List<Edge_Behaviour> EdgeBehaviours
    {
        get { return edgeBehaviours; }
        set { edgeBehaviours = value; }
    }


    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void CheckParents()
    {
        if (!this.gameObject.HasChildNamed("Nodes", out nodesParent))
            this.nodesParent = this.gameObject.CreateChild("Nodes");
        else

        if (!this.gameObject.HasChildNamed("Edges", out edgesParent))
            this.edgesParent = this.gameObject.CreateChild("Edges");
    }

    [SerializeField]
    public GameObject nodesParent;
    [SerializeField]
    public GameObject edgesParent;

    public void GenerateGameObjects()
    {
        this.gameObject.DeleteChildren();

        this.nodesParent = this.gameObject.CreateChild("Nodes");
        this.edgesParent = this.gameObject.CreateChild("Edges");

        foreach (Node node in this.graph.Nodes)
            CreateNodeBahaviour(node);

        foreach (Edge edge in this.graph.Edges)
            CreateEdgeBehaviour(edge);
    }



    public void AddNodeBehaviour(Node_Behaviour nodeBehaviour)
    {
        if (!this.nodeBehaviours.Contains(nodeBehaviour))
            this.nodeBehaviours.Add(nodeBehaviour);
    }

    public Node_Behaviour CreateNodeBehaviour(Vector3 position)
    {
        //Debug.Log("GraphBehaviour: Create Node at Position " + position);
        Node node = this.graph.CreateNode(position);

        Node_Behaviour nodeBehaviour = CreateNodeBahaviour(node);

        return nodeBehaviour;
    }

    private Node_Behaviour CreateNodeBahaviour(Node node)
    {
        //Debug.Log("GraphBehaviour: Create NodeBehaviour for Node " + node.Id);
        if (nodesParent == null)
            CheckParents();

        GameObject nodeGO = nodesParent.CreateChild(node.Id);
        Node_Behaviour nodeBehaviour = nodeGO.AddComponent<Node_Behaviour>();
        nodeBehaviour.InitializeBehaviour(this, node);

        AddNodeBehaviour(nodeBehaviour);
        return nodeBehaviour;
    }


    public void RemoveNodeBehaviour(Node_Behaviour nodeBehaviour)
    {
        //Debug.Log("Graphbehaviour RemoveNodeBehaviour : " + nodeBehaviour.name);
        this.nodeBehaviours.Remove(nodeBehaviour);
    }

    /// <summary>
    /// Returns a NodeBehaviour with the given ID (if exits).
    /// </summary>
    /// <param name="id">The ID of the desired node</param>
    /// <returns>A Node with the given ID (if exists), NULL if none available.</returns>
    public Node_Behaviour GetNodeBehaviourByID(string id)
    {
        // Iterate over all edges to find...
        foreach (Node_Behaviour nodeBehaviour in this.nodeBehaviours)
            // ... the node with the desired ID
            if (nodeBehaviour.Node.Id == id)
                return nodeBehaviour;

        // If the node has not been found, return NULL
        return null;
    }




    public void AddEdgeBehaviour(Edge_Behaviour edgeBehaviour)
    {
        if (!this.edgeBehaviours.Contains(edgeBehaviour))
            this.edgeBehaviours.Add(edgeBehaviour);
    }

    public void RemoveEdgeBehaviour(Edge_Behaviour edgeBehaviour)
    {
        //Debug.Log("Graphbehaviour RemoveEdge : " + edgeBehaviour.name);
        this.edgeBehaviours.Remove(edgeBehaviour);
    }

    //public void DeleteEdgeBehaviour(Edge_Behaviour edgeBehaviour)
    //{
    //    edgeBehaviour.Edge.Delete();
    //}


    public Edge_Behaviour CreateEdgeBehaviour(Node from, Node to)
    {
        //Debug.Log("GraphBehaviour: Going to create EdgeBehaviour for From-Node " + from.node.Id + " and To-Node " + to.node.Id);
        if (from == null || to == null)
            return null;

        Node_Behaviour fromBehaviour = GetNodeBehaviourByID(from.Id);
        if ( fromBehaviour == null)
            fromBehaviour = CreateNodeBahaviour(from);

        Node_Behaviour toBehaviour = GetNodeBehaviourByID(to.Id);
        if (toBehaviour == null)
            toBehaviour = CreateNodeBahaviour(to);

        return CreateEdgeBehaviour(fromBehaviour, toBehaviour);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public Edge_Behaviour CreateEdgeBehaviour(Node_Behaviour from, Node_Behaviour to)
    {
        //Debug.Log("GraphBehaviour: Going to create EdgeBehaviour for From-Node " + from.node.Id + " and To-Node " + to.node.Id);
        if (from == null || to == null)
            return null;

        Edge edge = this.graph.CreateEdge(from.Node, to.Node);

        if (edgesParent == null)
            CheckParents();

        //Debug.Log("GraphBehaviour: Create EdgeBehaviour for From-Node " + from.node.Id + " and To-Node " + to.node.Id);
        GameObject edgeGO = edgesParent.CreateChild(edge.Id);
        Edge_Behaviour edgeBehaviour = edgeGO.AddComponent<Edge_Behaviour>();
        edgeBehaviour.InitializeBehaviour(this, edge, from, to);

        AddEdgeBehaviour(edgeBehaviour);

        return edgeBehaviour;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    private Edge_Behaviour CreateEdgeBehaviour(Edge edge)
    {
        GameObject edgeGO = edgesParent.CreateChild(edge.Id);
        Edge_Behaviour edgeBehaviour = edgeGO.AddComponent<Edge_Behaviour>();
        edgeBehaviour.InitializeBehaviour(this, edge, GetNodeBehaviourByID(edge.From.Id), GetNodeBehaviourByID(edge.To.Id));

        AddEdgeBehaviour(edgeBehaviour);
        return edgeBehaviour;
    }


    /// <summary>
    /// Returns an edge with the given ID (if exists)
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns an edge with the given ID (if exists), NULL if none available.</returns>
    public Edge_Behaviour GetEdgeBehaviourByID(string id)
    {
        // Iterate over all edges to find...
        foreach (Edge_Behaviour edgeBehaviour in this.edgeBehaviours)
            // ... the edge with the desired ID
            if (edgeBehaviour.Edge.Id == id)
                return edgeBehaviour;

        // If the edge has not been found, return NULL
        return null;
    }



    /// <summary>
    /// Get the node Nodebehaviour with the minimal distance within the given Distance 
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Node_Behaviour GetNodeInDistance(Vector3 worldPoint, float distance)
    {
        Node_Behaviour result = null;
        float minDistance = float.MaxValue;
        foreach (Node_Behaviour nodeBehaviour in nodeBehaviours)
        {
            float tmp = (worldPoint - nodeBehaviour.Node.Position).magnitude;
            if (tmp < minDistance)
            {
                minDistance = tmp;
                result = nodeBehaviour;
            }
        }
        if (minDistance < distance)
            return result;
        else
            return null;
    }


    public List<Node_Behaviour> GetNodesInDistance(Vector3 worldPoint, float distance)
    {
        List<KeyValuePair<Node_Behaviour, float>> nodesInDistance = new List<KeyValuePair<Node_Behaviour, float>>();

        float quadDistance = distance * distance;

        foreach (Node_Behaviour nodeBehaviour in nodeBehaviours)
        {
            float tmp = (worldPoint - nodeBehaviour.Node.Position).sqrMagnitude;
            if (tmp < distance)
                nodesInDistance.Add(new KeyValuePair<Node_Behaviour, float>(nodeBehaviour, tmp));
        }

        nodesInDistance.Sort(
            delegate(KeyValuePair<Node_Behaviour, float> firstPair, KeyValuePair<Node_Behaviour, float> nextPair)
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
        );

        List<Node_Behaviour> result = new List<Node_Behaviour>();
        foreach (KeyValuePair<Node_Behaviour, float> item in nodesInDistance)
            result.Add(item.Key);

        return result;
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="worldpoint"></param>
    ///// <param name="distance"></param>
    ///// <returns></returns>
    //public Edge_Behaviour GetEdgeInDistance(Vector3 worldpoint, float distance)
    //{
    //    Edge_Behaviour result = null;

    //    float minDistance = float.MaxValue;
    //    Vector3 nearestPointOnEdge;

    //    foreach (Edge_Behaviour edgeBehaviour in edgeBehaviours)
    //    {
    //        Edge edge = edgeBehaviour.Edge;

    //        nearestPointOnEdge = MathUtils.GetMinDistancePointOnLine(
    //            edge.From.Position,
    //            edge.To.Position,
    //            worldpoint);

    //        float tmp = (worldpoint - nearestPointOnEdge).magnitude;
    //        if (tmp < minDistance)
    //        {
    //            minDistance = tmp;
    //            result = edgeBehaviour;
    //        }
    //    }

    //    if (minDistance < distance && result != null)
    //        return result;

    //    return null;
    //}



    /// <summary>
    /// 
    /// </summary>
    /// <param name="worldpoint"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Edge_Behaviour GetEdgeInScreenDistance(Vector3 worldpoint, float distance)
    {
        Edge_Behaviour result = null;

        float minDistance = float.MaxValue;
        Vector3 nearestPointOnEdge;

        foreach (Edge_Behaviour edgeBehaviour in edgeBehaviours)
        {
            Edge edge = edgeBehaviour.Edge;

            nearestPointOnEdge = MathUtils.GetMinDistancePointOnLine(
                edge.From.Position,
                edge.To.Position,
                worldpoint);

            float tmp = (worldpoint - nearestPointOnEdge).magnitude;
            if (tmp < minDistance)
            {
                minDistance = tmp;
                result = edgeBehaviour;
            }
        }

        if (minDistance < distance && result != null)
            return result;
        
        return null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Edge_Behaviour GetEdgeInDistance(Node_Behaviour node, float distance)
    {
        Edge_Behaviour result = null;

        float minDistance = float.MaxValue;
        Vector3 nearestPointOnEdge;

        foreach (Edge_Behaviour edgeBehaviour in edgeBehaviours)
        {
            Edge edge = edgeBehaviour.Edge;
            if (!node.Node.Edges.Contains(edge))
            {
                nearestPointOnEdge = MathUtils.GetMinDistancePointOnLine(
                    edge.From.Position,
                    edge.To.Position,
                    node.transform.position);

                float tmp = (node.transform.position - nearestPointOnEdge).sqrMagnitude;
                if (tmp < minDistance)
                {
                    minDistance = tmp;
                    result = edgeBehaviour;
                }
            }
        }

        if (minDistance < distance && result != null)
            return result;

        return null;
    }


    /// <summary>
    /// Returns The Edges in distance
    /// </summary>
    /// <param name="worldpoint"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<Edge_Behaviour> GetEdgesInDistance(Vector3 worldpoint, float distance)
    {
        List<KeyValuePair<Edge_Behaviour, float>> edgesInDistance = new List<KeyValuePair<Edge_Behaviour, float>>();
        //List<Edge_Behaviour> edgesInDistance = new List<Edge_Behaviour>();

        Vector3 nearestPointOnEdge;

        foreach (Edge_Behaviour edgeBehaviour in edgeBehaviours)
        {
            Edge edge = edgeBehaviour.Edge;

            nearestPointOnEdge = MathUtils.GetMinDistancePointOnLine(
                edge.From.Position,
                edge.To.Position,
                worldpoint);

            float tmp = (worldpoint - nearestPointOnEdge).magnitude;
            if (tmp < distance)
                edgesInDistance.Add(new KeyValuePair<Edge_Behaviour, float>(edgeBehaviour, tmp));
        }

        edgesInDistance.Sort(
            delegate(KeyValuePair<Edge_Behaviour, float> firstPair, KeyValuePair<Edge_Behaviour, float> nextPair)
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
        );

         List<Edge_Behaviour> result = new List<Edge_Behaviour>();
        foreach (KeyValuePair<Edge_Behaviour, float> item in edgesInDistance)
            result.Add(item.Key);

        return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="distance"></param>
    /// <param name="pointOnEdge"></param>
    /// <param name="pointOnEdgeParameter"></param>
    /// <returns></returns>
    public Edge_Behaviour GetEdgeInDistanceWithPoint(Node_Behaviour node, float distance, out Vector3 pointOnEdge, out float pointOnEdgeParameter)
    {

        Edge_Behaviour edgeBehaviour = GetEdgeInDistance(node, distance);
        pointOnEdge = Vector3.zero;
        pointOnEdgeParameter = float.NaN;

        if (edgeBehaviour != null)
        {
            pointOnEdge = MathUtils.GetMinDistancePointOnLine(
                edgeBehaviour.Edge.From.Position,
                edgeBehaviour.Edge.To.Position,
                node.transform.position,
                out pointOnEdgeParameter);
        }

        return edgeBehaviour;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="distance"></param>
    /// <param name="pointOnEdge"></param>
    /// <param name="pointOnEdgeParameter"></param>
    /// <returns></returns>
    public Edge_Behaviour GetEdgeInDistanceWithPoint(Vector3 mousePosition, float distance, out Vector3 pointOnEdge, out float pointOnEdgeParameter)
    {

        Edge_Behaviour edgeBehaviour = GetEdgeInScreenDistance(mousePosition, distance);
        pointOnEdge = Vector3.zero;
        pointOnEdgeParameter = float.NaN;

        if (edgeBehaviour != null)
        {
            pointOnEdge = MathUtils.GetMinDistancePointOnLine(
                edgeBehaviour.Edge.From.Position,
                edgeBehaviour.Edge.To.Position,
                mousePosition,
                out pointOnEdgeParameter);
        }

        return edgeBehaviour;
    }

    private List<Vector3> intersections = new List<Vector3>();

    
    
    
    public List<Vector3> CalculateIntersections(Node_Behaviour nodeBehaviour)
    {
        intersections.Clear();
        foreach (Edge edge in nodeBehaviour.Node.Edges)
        {
            foreach (Edge intersectingEdge in this.Graph.Edges)
            {
                IntersectionHelper intersection = IntersectionHelper.Line2LineIntersect(
                    edge.From.Position,
                    edge.To.Position,
                    intersectingEdge.From.Position,
                    intersectingEdge.To.Position);

                if (edge.From.Position == intersectingEdge.From.Position
                    || edge.From.Position == intersectingEdge.To.Position
                    || edge.To.Position == intersectingEdge.From.Position
                    || edge.To.Position == intersectingEdge.To.Position)
                {
                    continue;
                }

                if (intersection.Status == IntersectionStatus.COINCIDENT || intersection.Status == IntersectionStatus.PARALLEL)
                {

                }
                else if (intersection.Status == IntersectionStatus.INTERSECTION)
                {
                    intersections.AddRange(intersection.points);
                    Debug.Log("Intersections Found " + intersection.points.Count);
                }
            }
        }
        return intersections;
    }

    public List<Vector3> UpdateEdgeIntersections()
    {
        intersections.Clear();

        // TODO: Use SweepLine-Algorithm for fast calculation of all Intersections
        foreach (Edge edge in this.Graph.Edges)
        {
            foreach (Edge intersectingEdge in this.Graph.Edges)
            {
                IntersectionHelper intersection = IntersectionHelper.Line2LineIntersect(
                    edge.From.Position,
                    edge.To.Position,
                    intersectingEdge.From.Position,
                    intersectingEdge.To.Position);

                if (edge.From.Position == intersectingEdge.From.Position
                    || edge.From.Position == intersectingEdge.To.Position
                    || edge.To.Position == intersectingEdge.From.Position
                    || edge.To.Position == intersectingEdge.To.Position)
                {
                    continue;
                }

                if (intersection.Status == IntersectionStatus.COINCIDENT || intersection.Status == IntersectionStatus.PARALLEL)
                {
                    // TODO: Maybe merge the Lines
                }
                else if (intersection.Status == IntersectionStatus.INTERSECTION)
                {
                    intersections.AddRange(intersection.points);
                    //Debug.Log("Intersections Found " + intersection.points.Count);
                }
            }
        }
        return intersections;
    }


    


    public void CutEdgesAtIntersections()
    {
        //this.intersections = CalculateIntersections();
        //while (intersections.Count > 0)
        //{
        //    List<Edge> concurrentEdges = this.Graph.Edges;


        //    Intersection intersection = Intersection.Line2LineIntersect(
        //        edge.From.Position,
        //        edge.To.Position,
        //        intersectingEdge.From.Position,
        //        intersectingEdge.To.Position);

        //    if (edge.From.Position == intersectingEdge.From.Position
        //        || edge.From.Position == intersectingEdge.To.Position
        //        || edge.To.Position == intersectingEdge.From.Position
        //        || edge.To.Position == intersectingEdge.To.Position)
        //    {
        //        continue;
        //    }

        //    if (intersection.Status == IntersectionStatus.COINCIDENT || intersection.Status == IntersectionStatus.PARALLEL)
        //    {
        //        // TODO: Maybe merge the Lines
        //    }
        //    else if (intersection.Status == IntersectionStatus.INTERSECTION)
        //    {
        //        intersections.AddRange(intersection.points);
        //        //Debug.Log("Intersections Found " + intersection.points.Count);
        //    }

        //    // Calculate remaining Intersections
        //    this.intersections = CalculateIntersections();
        //}

    }


    public void OnDrawGizmos()
    {
        foreach (Vector3 intersectionPoint in intersections)
        {
            Gizmos.color = Color.red;
#if UNITY_EDITOR
            Gizmos.DrawSphere(intersectionPoint, HandleUtility.GetHandleSize(intersectionPoint) * 0.1f);
#else
            Gizmos.DrawSphere(intersectionPoint, 0.1f);
#endif
        }
    }

}

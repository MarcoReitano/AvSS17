using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class Node_Behaviour_Extensions
{

    public static void SnapToNode(this Node node, Node otherNode, float treshold)
    {
        Vector3 position = new Vector3(otherNode.Position.x, node.Position.y, otherNode.Position.z);

        if ((position - node.Position).magnitude < treshold)
            node.Position = position;
    }

    public static void SnapToEdge(this Node selectedNode, Edge_Behaviour foundEdge, ref Vector3 pointOnEdge, ref float pointOnEdgeParameter, float treshold)
    {
        //Debug.Log("Snap selected Node(" + selectedNode.name + ") to Edge(" + foundEdge.name + ") at " + pointOnEdge);
        pointOnEdge = MathUtils.GetMinDistancePointOnLine(
            foundEdge.Edge.From.Position,
            foundEdge.Edge.To.Position,
            selectedNode.Position,
            out pointOnEdgeParameter);

        if ((pointOnEdge - selectedNode.Position).magnitude < treshold)
            selectedNode.Position = pointOnEdge;
    }

    public static void SnapToEdge(this Node node, Edge edge, float treshold)
    {
        //Debug.Log("Snap selected Node(" + selectedNode.name + ") to Edge(" + foundEdge.name + ") at " + pointOnEdge);
        Vector3 pointOnEdge = MathUtils.GetMinDistancePointOnLine(
            edge.From.Position,
            edge.To.Position,
            node.Position);

        pointOnEdge.y = node.Position.y;

        if ((pointOnEdge - node.Position).magnitude < treshold)
            node.Position = pointOnEdge;
    }


    public static void SnapToHelpLines(this Node nodeToSnap, float treshold)
    {

        foreach (Edge edge in nodeToSnap.Graph.Edges)
        {
            // Verlängerung der Edge
            nodeToSnap.SnapToHelpLine(edge.From.Position, edge.To.Position, treshold);
            // Orthogonal zur Edge am from-node
            nodeToSnap.SnapToHelpLine(edge.From.Position, edge.From.Position - edge.OrthoNormalVector, treshold);
            // Orthogonal zur Edge am to-node
            nodeToSnap.SnapToHelpLine(edge.To.Position, edge.To.Position - edge.OrthoNormalVector, treshold);
        }

        foreach (Node node in nodeToSnap.Graph.Nodes)
        {
            nodeToSnap.SnapToHelpLine(node.Position, node.Position + Vector3.forward, treshold);
            nodeToSnap.SnapToHelpLine(node.Position, node.Position + Vector3.right, treshold);
        }
    }

    public static void SnapToHelpLine(this Node nodeToSnap, Vector3 P0, Vector3 P1, float threshold)
    {
        Vector3 pointOnRay = MathUtils.GetMinDistancePointOnRay(P0, P1, nodeToSnap.Position);
        if (pointOnRay.InDistance(nodeToSnap.Position, threshold))
            nodeToSnap.Position = pointOnRay;
    }


    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="node"></param>
    ///// <param name="edge"></param>
    ///// <param name="pointOnEdge"></param>
    //public static void SnapToEdgeCutting(this Node node, Edge_Behaviour edge, Vector3 pointOnEdge, Floor floor)
    //{
    //    if (edge != null && node != null)
    //        if (node.Edges.Contains(edge.Edge))
    //            return;

    //    Edge otherEdge;
    //    Node_Behaviour nodeBehaviour = edge.GraphBehaviour.GetNodeBehaviourByID(node.Id);
    //    if (!node.IsConnectedTo(edge.FromBehaviour.Node, out otherEdge))
    //    {
    //        // Create new Edge for first part   
    //        Edge_Behaviour cutEdge1 = edge.GraphBehaviour.CreateEdgeBehaviour(edge.FromBehaviour, nodeBehaviour);
    //        edge.GraphBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(cutEdge1, floor);
    //    }

    //    if (!node.IsConnectedTo(edge.ToBehaviour.Node, out otherEdge))
    //    {
    //        // Create new Edge for second part
    //        Edge_Behaviour cutEdge2 = edge.GraphBehaviour.CreateEdgeBehaviour(nodeBehaviour, edge.ToBehaviour);
    //        edge.GraphBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(cutEdge2, floor);
    //    }

    //    edge.Edge.Delete();
    //    node.OnNodeChanged();
    //}


    //public static void DrawSelectedNode(this Node node, Color color)
    //{
    //    if (node != null)
    //    {
    //        if (IEMEditor.Is2DMode())
    //        {
    //            //NodeUtils.DrawCircle(node.Position + (Vector3.down * 1f), 0.8f, color);
    //        }
    //        else
    //            DebugDraw.DrawSphere(node.Position, 0.8f, color);
    //        //DebugDraw.DrawDisc(node.Position, Quaternion.identity, 0.8f, color);
    //    }
    //}


    //public static Node ConnectWithNode(this Node start, Node end, Floor_Behaviour floor)
    //{
    //    if (!start.IsConnectedTo(end))
    //    {
    //        Edge_Behaviour newEdgeBehaviour = floor.buildingBehaviour.RealEstateBehaviour.IemBehaviour.GraphBehaviour.CreateEdgeBehaviour(start, end);
    //        floor.buildingBehaviour.RealEstateBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(newEdgeBehaviour, floor.Floor);
    //    }
    //    return end;
    //}

    //public static Node ConnectWithNewNode(this Node selectedNode, Vector3 worldPoint, Floor_Behaviour floor)
    //{
    //    Node newNode = floor.buildingBehaviour.RealEstateBehaviour.IemBehaviour.GraphBehaviour.CreateNodeBehaviour(worldPoint).Node;
    //    Edge_Behaviour newEdge = floor.buildingBehaviour.RealEstateBehaviour.IemBehaviour.GraphBehaviour.CreateEdgeBehaviour(selectedNode, newNode);
    //    floor.buildingBehaviour.RealEstateBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(newEdge, floor.Floor);

    //    return newNode;
    //}



    //public static Node CutEdgeWithNode(this Edge_Behaviour foundEdge, Vector3 pointOnEdge, Node selectedNode, float ThresholdDistance, Floor floor)
    //{

    //    //if (foundEdge != null && selectedNode != null)
    //    //    if (selectedNode.Edges.Contains(foundEdge.Edge))
    //    //        return selectedNode;


    //    // Create new Node at the cutting-point on the Edge
    //    Node_Behaviour newNode = foundEdge.GraphBehaviour.CreateNodeBehaviour(pointOnEdge);
    //    newNode.Node.SnapToHelpLines(ThresholdDistance);


    //    // Create new Edge for first part
    //    Edge_Behaviour cutEdge1 = foundEdge.GraphBehaviour.CreateEdgeBehaviour(foundEdge.FromBehaviour, newNode);
    //    foundEdge.GraphBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(cutEdge1, floor);

    //    // Create new Edge for second part
    //    Edge_Behaviour cutEdge2 = foundEdge.GraphBehaviour.CreateEdgeBehaviour(newNode, foundEdge.ToBehaviour);
    //    foundEdge.GraphBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(cutEdge2, floor);

    //    if (foundEdge.FromBehaviour.Node != selectedNode && foundEdge.ToBehaviour.Node != selectedNode)
    //    {
    //        // Create new Edge from the lastNode (this) to the new Node
    //        Edge_Behaviour newEdgeBehaviour = foundEdge.GraphBehaviour.CreateEdgeBehaviour(foundEdge.GraphBehaviour.GetNodeBehaviourByID(selectedNode.Id), newNode);
    //        foundEdge.GraphBehaviour.IemBehaviour.PrefabBehaviour.CreateWall(newEdgeBehaviour, floor);
    //    }

    //    foundEdge.Edge.Delete();

    //    newNode.Node.OnNodeChanged();
    //    return newNode.Node;
    //}


    //public static void SnapToFloorPoints(this Node nodeToSnap, Floor floor, float treshold)
    //{
    //    if (floor != null)
    //    {
    //        // Snap To Edges
    //        foreach (Edge edge in floor.GetEdges())
    //            nodeToSnap.SnapToEdge(edge, treshold);

    //        // Get Nodes of Floor
    //        foreach (Node node in floor.GetNodes())
    //            nodeToSnap.SnapToNode(node, treshold);
    //    }
    //}
}


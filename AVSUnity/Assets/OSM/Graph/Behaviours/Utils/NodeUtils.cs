using System.Globalization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class NodeUtils
{

    //private static Color GetColor(int i, float angle)
    //{
    //    //TODO: schönere Farben...  :o/
    //    if (angle == 90f)
    //        return Color.white;
    //    else if (angle == 180f)
    //        return Color.black;

    //    switch (i % 8)
    //    {
    //        case 0: return Color.blue;
    //        case 1: return Color.yellow;
    //        case 2: return Color.red;
    //        case 3: return Color.green;
    //        case 4: return Color.magenta;
    //        case 5: return Color.cyan;
    //        case 6: return Color.gray;
    //        case 7: return Color.grey;
    //        default:
    //            return Color.blue;
    //    }
    //}

    private static Color GetColor(int i, float angle)
    {
        //TODO: schönere Farben...  :o/
        //if (angle == 90f)
        //    return Color.white;
        //else if (angle == 180f)
        //    return Color.black;

        float factor = angle / 180;
        return new Color(factor, factor, factor, 1f);
    }



    public static void DrawAnglePieGizmo(Node node)
    {
        DrawAnglePieGizmo(node, true, 0f);
    }


    public static void DrawAnglePieGizmo(Node node, float anglePieRadius)
    {
        DrawAnglePieGizmo(node, false, anglePieRadius);
    }

    public static void DrawAnglePieGizmo(Node node, bool anglePieRadiusDynamic, float anglePieRadius)
    {
#if UNITY_EDITOR

        // Get the edges
        Edge[] edgesArray = node.Edges.ToArray();

        // continue if the node is not a dead end
        if (edgesArray.Length > 1)
        {
            // calculate the radius
            anglePieRadius = calculateRadius(node, anglePieRadiusDynamic, anglePieRadius);
            float alpha = 1;

            // handle all edges
            for (int i = 0; i < edgesArray.Length; i++)
            {
                Edge edge1 = (Edge)edgesArray[i];
                Edge edge2;

                if (i == edgesArray.Length - 1) // last Edge
                {
                    edge2 = (Edge)edgesArray[0];

                    float angle = Vector3.Angle(edge1.GetLookDirectionFromNode(node), edge2.GetLookDirectionFromNode(node));
                    float angleToZ = MathUtils.AngleToZAxis(edge1, node) - MathUtils.AngleToZAxis(edge2, node);

                    Plane plane = new Plane(node.Position, node.Position + edge1.GetLookDirectionFromNode(node), node.Position + edge2.GetLookDirectionFromNode(node));
                    Vector3 normal = plane.normal;
                    if (normal == Vector3.zero)
                        normal = Vector3.up;

                    if (angleToZ < 180)
                    {
                        angle = -(360 - angle);
                        continue;
                    }

                    Color test = GetColor(i, Mathf.Abs(angle));
                    Handles.color = new Color(test.r, test.g, test.b, alpha);
                    Handles.DrawSolidArc(node.Position, normal, edge2.GetLookDirectionFromNode(node), -angle, anglePieRadius * 360);

                    Vector3 winkelhalbierende = (edge1.GetLookDirectionFromNode(node).normalized + edge2.GetLookDirectionFromNode(node).normalized).normalized * (anglePieRadius * 360);

                    if (Mathf.Abs(angle) < 180)
                    {
                        if (Mathf.Abs(angle) == 90)
                        {
                            Handles.color = Color.black;
                            Handles.DrawSolidArc(node.Position + (winkelhalbierende * 0.5f), normal, Vector3.forward, 360, anglePieRadius * 10);
                        }
                        else
                            Handles.Label(node.Position + (winkelhalbierende * 0.6f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
                    }
                    else
                        Handles.Label(node.Position - (winkelhalbierende * 0.4f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
                }
                else
                {
                    // next edge
                    edge2 = (Edge)edgesArray[i + 1];

                    float angle = Vector3.Angle(edge1.GetLookDirectionFromNode(node), edge2.GetLookDirectionFromNode(node));
                    float angleToZ = MathUtils.AngleToZAxis(edge2, node) - MathUtils.AngleToZAxis(edge1, node);

                    Plane plane = new Plane(node.Position, node.Position + edge1.GetLookDirectionFromNode(node), node.Position + edge2.GetLookDirectionFromNode(node));
                    Vector3 normal = plane.normal;
                    if (normal == Vector3.zero)
                        normal = -Vector3.up;

                    if (angleToZ >= 180)
                    {
                        angle = -(360 - angle);
                        continue;
                    }

                    Color test = GetColor(i, Mathf.Abs(angle));
                    Handles.color = new Color(test.r, test.g, test.b, alpha);
                    Handles.DrawSolidArc(node.Position, normal, edge1.GetLookDirectionFromNode(node), angle, anglePieRadius * 360);

                    Vector3 winkelhalbierende = (edge1.GetLookDirectionFromNode(node).normalized + edge2.GetLookDirectionFromNode(node).normalized).normalized * (anglePieRadius * 360);
                    if (Mathf.Abs(angle) < 180)
                    {
                        if (Mathf.Abs(angle) == 90)
                        {
                            Handles.color = Color.black;
                            Handles.DrawSolidArc(node.Position + (winkelhalbierende * 0.5f), normal, Vector3.forward, 360, anglePieRadius * 20);
                        }
                        else
                            Handles.Label(node.Position + (winkelhalbierende * 0.6f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
                    }
                    else
                        Handles.Label(node.Position - (winkelhalbierende * 0.4f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
                }
            }
        }
#endif
    }

    private static float calculateRadius(Node node, bool anglePieRadiusDynamic, float anglePieRadius)
    {
        float minEdgeLength = node.MinEdgeLength();

        if (anglePieRadiusDynamic)
            anglePieRadius = (minEdgeLength / 4) / 360;
        else
            anglePieRadius /= 360;
        return anglePieRadius;
    }



    //private static CustomMesh circleCustomMesh;
    //private static Mesh circleMesh;
    //private static float oldRadius = Mathf.Infinity;
    //private static Material circleMeshMaterial;
    //private static Material circleMeshMaterialRed = new Material(Shader.Find("Diffuse"));
    //private static Material circleMeshMaterialYellow = new Material(Shader.Find("Diffuse"));
    //private static Material circleMeshMaterialGreen = new Material(Shader.Find("Diffuse"));
    //private static Material circleMeshMaterialBlue = new Material(Shader.Find("Diffuse"));
    //private static Material circleMeshMaterialGrey = new Material(Shader.Find("Diffuse"));


    //// TODO: Hier sollten nur einemal die Meshes erstellt werden von einem Radius und einer Farbe.
    //// Sonst werden ständig neue Meshes angelegt bei wechselndem aufruf
    //public static void DrawCircle(Vector3 center, float radius, Color color)
    //{
    //    //DebugDraw.DrawDisc(center, Quaternion.identity, radius, color);
    //    DebugDraw.DrawQuad(center, Quaternion.identity, radius, color);

    //    //circleMeshMaterialRed.color = Color.red;
    //    //circleMeshMaterialYellow.color = Color.yellow;
    //    //circleMeshMaterialGreen.color = Color.green;
    //    //circleMeshMaterialBlue.color = Color.blue;
    //    //circleMeshMaterialGrey.color = Color.grey;


    //    //if (radius != oldRadius)
    //    //{
    //    //    oldRadius = radius;
    //    //    circleCustomMesh = new CustomMesh();

    //    //    Vector3 P1, P2;
    //    //    P1 = MathUtils.PointOnCircleXZ(Vector3.zero, radius, 0);

    //    //    Vertex V_P0 = new Vertex(Vector3.zero);
    //    //    Vertex V_P1 = new Vertex(P1);

    //    //    for (int i = 0; i <= 360; i += 10)
    //    //    {
    //    //        P2 = MathUtils.PointOnCircleXZ(Vector3.zero, radius, i);
    //    //        Vertex V_P2 = new Vertex(P2);

    //    //        circleCustomMesh.AddTriangle(V_P0, V_P2, V_P1);
    //    //        V_P1 = V_P2;
    //    //    }

    //    //    circleMesh = circleCustomMesh.CompileToUnityMesh();
    //    //}

    //    //if (color == Color.red)
    //    //    circleMeshMaterial = circleMeshMaterialRed;
    //    //else if (color == Color.yellow)
    //    //    circleMeshMaterial = circleMeshMaterialYellow;
    //    //else if (color == Color.green)
    //    //    circleMeshMaterial = circleMeshMaterialGreen;
    //    //else if (color == Color.blue)
    //    //    circleMeshMaterial = circleMeshMaterialBlue;
    //    //else if (color == Color.grey)
    //    //    circleMeshMaterial = circleMeshMaterialGrey;
    //    //else
    //    //    circleMeshMaterial = circleMeshMaterialRed;


    //    //Graphics.DrawMesh(circleMesh, center, Quaternion.identity, circleMeshMaterial, 0);
    //}


    //public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color)
    //{
    //    circleMeshMaterialRed.color = Color.red;
    //    circleMeshMaterialYellow.color = Color.yellow;
    //    circleMeshMaterialGreen.color = Color.green;
    //    circleMeshMaterialBlue.color = Color.blue;
    //    circleMeshMaterialGrey.color = Color.grey;


    //    if (radius != oldRadius)
    //    {
    //        oldRadius = radius;
    //        circleCustomMesh = new CustomMesh();

    //        float startAngle = MathUtils.AngleToZAxis(from);
    //        float endAngle = startAngle + angle;
    //        float angleStep = angle / 16f;


    //        Vector3 P1, P2;
    //        P1 = MathUtils.PointOnCircleXZ(Vector3.zero, radius, startAngle);

    //        Vertex V_P0 = new Vertex(Vector3.zero);
    //        Vertex V_P1 = new Vertex(P1);

    //        for (float i = startAngle; i <= startAngle + angle; i += angleStep)
    //        {
    //            P2 = MathUtils.PointOnCircleXZ(Vector3.zero, radius, i);
    //            Vertex V_P2 = new Vertex(P2);

    //            circleCustomMesh.AddTriangle(V_P0, V_P2, V_P1);
    //            V_P1 = V_P2;
    //        }

    //        circleMesh = circleCustomMesh.CompileToUnityMesh();
    //    }

    //    if (color == Color.red)
    //        circleMeshMaterial = circleMeshMaterialRed;
    //    else if (color == Color.yellow)
    //        circleMeshMaterial = circleMeshMaterialYellow;
    //    else if (color == Color.green)
    //        circleMeshMaterial = circleMeshMaterialGreen;
    //    else if (color == Color.blue)
    //        circleMeshMaterial = circleMeshMaterialBlue;
    //    else
    //        circleMeshMaterial = circleMeshMaterialRed;


    //    Graphics.DrawMesh(circleMesh, center, Quaternion.identity, circleMeshMaterial, 0);
    //}

       


    //public static void DrawAnglePie(Node node, bool anglePieRadiusDynamic, float anglePieRadius)
    //{

    //    // Get the edges
    //    Edge[] edgesArray = node.Edges.ToArray();

    //    // continue if the node is not a dead end
    //    if (edgesArray.Length > 1)
    //    {
    //        // calculate the radius
    //        anglePieRadius = calculateRadius(node, anglePieRadiusDynamic, anglePieRadius);
    //        float alpha = 1;

    //        // handle all edges
    //        for (int i = 0; i < edgesArray.Length; i++)
    //        {
    //            Edge edge1 = (Edge)edgesArray[i];
    //            Edge edge2;

    //            if (i == edgesArray.Length - 1) // last Edge
    //            {
    //                edge2 = (Edge)edgesArray[0];

    //                float angle = Vector3.Angle(edge1.GetLookDirectionFromNode(node), edge2.GetLookDirectionFromNode(node));
    //                float angleToZ = MathUtils.AngleToZAxis(edge1, node) - MathUtils.AngleToZAxis(edge2, node);

    //                Plane plane = new Plane(node.Position, node.Position + edge1.GetLookDirectionFromNode(node), node.Position + edge2.GetLookDirectionFromNode(node));
    //                Vector3 normal = plane.normal;
    //                if (normal == Vector3.zero)
    //                    normal = Vector3.up;

    //                if (angleToZ < 180)
    //                {
    //                    angle = -(360 - angle);
    //                    continue;
    //                }

    //                Color test = GetColor(i, Mathf.Abs(angle));
    //                DrawSolidArc(node.Position, normal, edge2.GetLookDirectionFromNode(node), -angle, anglePieRadius * 360, new Color(test.r, test.g, test.b, alpha));

    //                Vector3 winkelhalbierende = (edge1.GetLookDirectionFromNode(node).normalized + edge2.GetLookDirectionFromNode(node).normalized).normalized * (anglePieRadius * 360);

    //                if (Mathf.Abs(angle) < 180)
    //                {
    //                    if (Mathf.Abs(angle) == 90)
    //                    {
    //                        DrawSolidArc(node.Position + (winkelhalbierende * 0.5f), normal, Vector3.forward, 360, anglePieRadius * 10, Color.black);
    //                    }
    //                    else
    //                        CustomGUIUtils.Label(node.Position + (winkelhalbierende * 0.6f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
    //                }
    //                else
    //                    CustomGUIUtils.Label(node.Position - (winkelhalbierende * 0.4f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
    //            }
    //            else
    //            {
    //                // next edge
    //                edge2 = (Edge)edgesArray[i + 1];

    //                float angle = Vector3.Angle(edge1.GetLookDirectionFromNode(node), edge2.GetLookDirectionFromNode(node));
    //                float angleToZ = MathUtils.AngleToZAxis(edge2, node) - MathUtils.AngleToZAxis(edge1, node);

    //                Plane plane = new Plane(node.Position, node.Position + edge1.GetLookDirectionFromNode(node), node.Position + edge2.GetLookDirectionFromNode(node));
    //                Vector3 normal = plane.normal;
    //                if (normal == Vector3.zero)
    //                    normal = -Vector3.up;

    //                if (angleToZ >= 180)
    //                {
    //                    angle = -(360 - angle);
    //                    continue;
    //                }

    //                Color test = GetColor(i, Mathf.Abs(angle));
    //                DrawSolidArc(node.Position, normal, edge1.GetLookDirectionFromNode(node), angle, anglePieRadius * 360, new Color(test.r, test.g, test.b, alpha));

    //                Vector3 winkelhalbierende = (edge1.GetLookDirectionFromNode(node).normalized + edge2.GetLookDirectionFromNode(node).normalized).normalized * (anglePieRadius * 360);
    //                if (Mathf.Abs(angle) < 180)
    //                {
    //                    if (Mathf.Abs(angle) == 90)
    //                    {
    //                        DrawSolidArc(node.Position + (winkelhalbierende * 0.5f), normal, Vector3.forward, 360, anglePieRadius * 20, Color.black);
    //                    }
    //                    else
    //                        CustomGUIUtils.Label(node.Position + (winkelhalbierende * 0.6f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
    //                }
    //                else
    //                    CustomGUIUtils.Label(node.Position - (winkelhalbierende * 0.4f), Mathf.Abs(angle).ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")) + "°");
    //            }
    //        }
    //    }
    //}

}


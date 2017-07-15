
using System.Text;
using UnityEngine;
using System.Collections.Generic;


public enum IntersectionStatus
{
    NO_INTERSECTION,        // keine Schnittpunkte
    INTERSECTION,           // ein Schnittpunkt
    MULTIPLE_INTERSECTIONS, // mehrere Schnittpunkte
    COINCIDENT,             // deckungsgleich
    PARALLEL                // parallel
}


public class IntersectionHelper
{

    private IntersectionStatus status;
    public IntersectionStatus Status
    {
        get { return status; }
        set { this.status = value; }
    }


    public List<Vector3> points;

    /// <summary>
    /// The Constructor
    /// </summary>
    public IntersectionHelper()
    {
        this.status = IntersectionStatus.NO_INTERSECTION;
        this.points = new List<Vector3>();
    }


    /// <summary>
    /// Appends an intersection-point to the intersections-list
    /// </summary>
    /// <param name="point"></param>
    public void AppendPoint(Vector3 point)
    {
        this.points.Add(point);
        CheckStatus();
    }

    private void CheckStatus()
    {
        if (this.points.Count == 0)
            this.status = IntersectionStatus.NO_INTERSECTION;
        if (this.points.Count == 1)
            this.status = IntersectionStatus.INTERSECTION;
        else
            this.status = IntersectionStatus.MULTIPLE_INTERSECTIONS;
    }


    /// <summary>
    /// Appends a list of intersection-points to the intersections-list
    /// </summary>
    /// <param name="points"></param>
    public void AppendPoints(List<Vector3> points)
    {
        foreach (Vector3 point in points)
        {
            if (!this.points.Contains(point))
            {
                this.points.Add(point);
            }
        }
        CheckStatus();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (this.Status == IntersectionStatus.NO_INTERSECTION)
        {
            sb.Append("No Intersection");
        }
        else if (this.Status == IntersectionStatus.INTERSECTION)
        {
            sb.Append("Intersection!!!");
            sb.Append("\n");
            sb.Append(points[0].ToString());
        }
        else if (this.Status == IntersectionStatus.MULTIPLE_INTERSECTIONS)
        {
            sb.Append("Multiple Intersections");
            foreach (Vector3 vect in this.points)
            {
                sb.Append("\n");
                sb.Append(vect.ToString());
            }
        }
        else if (this.Status == IntersectionStatus.COINCIDENT)
        {
            sb.Append("Coincident");
        }
        else if (this.Status == IntersectionStatus.PARALLEL)
        {
            sb.Append("Parallel");
        }


        return sb.ToString();
    }


    public static IntersectionHelper Line2LineIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        IntersectionHelper intersectionPoints = new IntersectionHelper();
        float nx, ny, dn;
        float x4_x3 = p4.x - p3.x;
        float pre2 = p4.z - p3.z;
        float pre3 = p2.x - p1.x;
        float pre4 = p2.z - p1.z;
        float pre5 = p1.z - p3.z;
        float pre6 = p1.x - p3.x;

        nx = x4_x3 * pre5 - pre2 * pre6;
        ny = pre3 * pre5 - pre4 * pre6;
        dn = pre2 * pre3 - x4_x3 * pre4;

        if (dn == 0)
        {
            //Debug.Log(p1 + p2 + p3 + p4 + " --> Dn == 0");
            return intersectionPoints;
        }

        nx /= dn;
        ny /= dn;

        // has intersection
        if (nx >= 0 && nx <= 1 && ny >= 0 && ny <= 1)
        {
            ny = p1.z + nx * pre4;
            nx = p1.x + nx * pre3;
            intersectionPoints.AppendPoint(new Vector3(nx, 0f, ny));
        }
        else
        {

        }
        return intersectionPoints;
    }





    /// <summary>
    /// 
    /// </summary>
    /// <param name="_a1"></param>
    /// <param name="_a2"></param>
    /// <param name="_b1"></param>
    /// <param name="_b2"></param>
    /// <returns></returns>
    public static IntersectionHelper Ray2RayIntersect(Vector3 _a1, Vector3 _a2, Vector3 _b1, Vector3 _b2)
    {

        Vector3 tmp = (_a1 - _a2) * 1000;
        _a1 = _a1 - tmp;
        _a2 = _a2 + tmp;

        Vector3 tmp2 = (_b1 - _b2) * 1000;
        _b1 = _b1 - tmp2;
        _b2 = _b2 + tmp2;

        Vector2 a1 = MathUtils.Vector3ToVector2IgnoringY(_a1);
        Vector2 a2 = MathUtils.Vector3ToVector2IgnoringY(_a2);
        Vector2 b1 = MathUtils.Vector3ToVector2IgnoringY(_b1);
        Vector2 b2 = MathUtils.Vector3ToVector2IgnoringY(_b2);


        IntersectionHelper result = new IntersectionHelper();

        float ua_t = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
        float ub_t = (a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x);
        float u_b = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);

        if (u_b != 0)
        {
            var ua = ua_t / u_b;
            var ub = ub_t / u_b;

            if (0 <= ua && ua <= 1 && 0 <= ub && ub <= 1)
            {
                result.AppendPoint(
                    MathUtils.Vector2ToVector3XZ(new Vector2(
                        a1.x + ua * (a2.x - a1.x),
                        a1.y + ua * (a2.y - a1.y)))
                );
            }
        }
        else
        {
            if (ua_t == 0 || ub_t == 0)
                result.Status = IntersectionStatus.COINCIDENT;
            else
                result.Status = IntersectionStatus.PARALLEL;
        }


        return result;
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="P1"></param>
    ///// <param name="P2"></param>
    ///// <param name="P3"></param>
    ///// <param name="P4"></param>
    ///// <returns></returns>
    //public static Intersection Ray2RayIntersect(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
    //{

    //    Vector2 _p1 = MathUtils.Vector3ToVector2IgnoringY(P1);
    //    Vector2 _p2 = MathUtils.Vector3ToVector2IgnoringY(P2);
    //    Vector2 _p3 = MathUtils.Vector3ToVector2IgnoringY(P3);
    //    Vector2 _p4 = MathUtils.Vector3ToVector2IgnoringY(P4);

    //    Vector2 A = _p2 - _p1;
    //    Vector2 B = _p3 - _p4;
    //    Vector2 C = _p1 - _p3;

    //    float ByCx = B.y * C.x;
    //    float BxCy = B.x * C.y;
    //    float AyBx = A.y * B.x;
    //    float AxBy = A.x * B.y;
    //    float AxCy = A.x * C.y;
    //    float AyCx = A.y * C.x;

    //    float AyBx_Minus_AxBy = AyBx - AxBy;

    //    float alpha = (ByCx - BxCy) / AyBx_Minus_AxBy;
    //    float beta = (AxCy - AyCx) / AyBx_Minus_AxBy;


    //    Intersection result = new Intersection();

    //    result.AppendPoint(
    //        MathUtils.Vector2ToVector3XZ(
    //            _p1 + alpha * (_p2 - _p1)
    //            ));


    //    //if (u_b != 0)
    //    //{
    //    //    var ua = ua_t / u_b;
    //    //    var ub = ub_t / u_b;

    //    //    if (0 <= ua && ua <= 1 && 0 <= ub && ub <= 1)
    //    //    {
    //    //        result.AppendPoint(
    //    //            MathUtils.Vector2ToVector3XZ(new Vector2(
    //    //                _p1.x + ua * (_p2.x - _p1.x),
    //    //                _p1.y + ua * (_p2.y - _p1.y)))
    //    //        );
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    if (ua_t == 0 || ub_t == 0)
    //    //        result.Status = IntersectionStatus.COINCIDENT;
    //    //    else
    //    //        result.Status = IntersectionStatus.PARALLEL;
    //    //}


    //    return result;
    //}

}


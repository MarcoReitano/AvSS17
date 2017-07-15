using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OSMTileBehaviour : MonoBehaviour
{
    [SerializeField]
    public OSMTile tile;
    [SerializeField]
    public List<Vector3> corners = new List<Vector3>();
    [SerializeField]
    public List<Vector3> cutCorners = new List<Vector3>();

    [SerializeField]
    public Mesh mesh;

    // Use this for initialization
    void Start()
    {
        if (this.tile == null)
            this.tile = OSMTile.GetOSMTile(TileManager.OriginLongitude, TileManager.OriginLatitude, 13);

    }


    public static float height = 5f;

    public void Initialize()
    {
        //
        //Debug.Log("Tile == NULL ?" + (tile == null));
        List<Vector3> positions = this.tile.MapBounds.WorldCoordsCorners();

        Vector3 origin = GeographicCoordinates.ConvertLonLatToXY(TileManager.OriginLongitude, TileManager.OriginLatitude, 0f).Vector3FromVector2XZ();
        Vector3 heightVector = new Vector3(0f, height, 0f);
        corners.Add(positions[0] - origin + heightVector);
        corners.Add(positions[1] - origin + heightVector);
        corners.Add(positions[2] - origin + heightVector);
        corners.Add(positions[3] - origin + heightVector);

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();



        //Debug.Log("Origin : " + origin);
        //Debug.Log("Position0 : " + positions[0]);
        mesh = new Mesh();
        //mesh.CreateQuad(positions[0] - origin, positions[1] - origin, positions[2] - origin, positions[3] - origin);
        mesh.CreateQuad(corners[0], corners[1], corners[2], corners[3]);
        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        meshRenderer.sharedMaterial.mainTexture = tile.Image;
        //transform.position = positions[0] - origin;

        //Debug.Log(tile.ToString());
    }

    public void InitializeCuttingBounds(MapBounds bounds)
    {
        //
        //Debug.Log("Tile == NULL ?" + (tile == null));
        List<Vector3> positions = this.tile.MapBounds.WorldCoordsCorners();

        Vector3 origin = GeographicCoordinates.ConvertLonLatToXY(TileManager.OriginLongitude, TileManager.OriginLatitude, 0f).Vector3FromVector2XZ();

        //Vector3 NWCorner =  


        corners.Add(positions[0] - origin);
        corners.Add(positions[1] - origin);
        corners.Add(positions[2] - origin);
        corners.Add(positions[3] - origin);

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

        Vector2 uv0 = new Vector2(0, 0);
        Vector2 uv1 = new Vector2(0, 1);
        Vector2 uv2 = new Vector2(1, 0);
        Vector2 uv3 = new Vector2(1, 1);


        //Debug.Log("Origin : " + origin);
        //Debug.Log("Position0 : " + positions[0]);
        mesh = new Mesh();

        mesh.CreateQuad(corners[0], corners[1], corners[2], corners[3], uv0, uv1, uv2, uv3);

        meshFilter.mesh = mesh;
        meshRenderer.sharedMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        meshRenderer.sharedMaterial.mainTexture = tile.Image;
        //transform.position = positions[0] - origin;

        //Debug.Log(tile.ToString());
    }




    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmosSelected()
    {
        //if (tile.ZoomLevel == OSMTileProviderBehaviour.CurrentZoomLevel)
        //{
        //    foreach (Vector3 pos in corners)
        //    {
        //        Gizmos.DrawSphere(pos, 100);
        //    }
        //}
    }

    void OnGUI()
    {
        //if (tile != null)
        //{
        //    if (tile.Image != null)
        //        GUI.DrawTexture(new Rect(0, 0, 256, 256), tile.Image);
        //}
    }
}

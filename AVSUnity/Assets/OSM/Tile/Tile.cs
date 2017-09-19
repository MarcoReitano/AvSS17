using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

/// <summary>
/// OSMTile.
/// </summary>
public class
    Tile : MonoBehaviour
{
    public static Tile CreateTileGO(int x, int z, int lod)
    {
        GameObject go = new GameObject("Tile" + x.ToString() + z.ToString());
        Tile tile = go.AddComponent<Tile>();
        tile.LOD = lod;
        tile.TileIndex = new int[2] { x, z };
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshCollider>();
        Console.AddMessage("Created Tile with LOD" + tile.lOD);
        return tile;
    }

    public int[] TileIndex = new int[2] { 0, 0 };
    public Vector3 TilePosition
    {
        get
        {
            return new Vector3
                      (
                          (float)(TileManager.TileWidth * TileIndex[0] * TileManager.Scaling),
                          0f,
                          (float)(TileManager.TileWidth * TileIndex[1] * TileManager.Scaling)
                      );
        }
    }

    public int LOD
    {
        get { return lOD; }
        set { lOD = Mathf.Clamp(value, 0, 5); }
    }
    [SerializeField]
    private int lOD = 5;

    
    [System.NonSerialized]
    public OverpassQuery Query;

    [System.NonSerialized]
    public ModularMesh TileMesh = new ModularMesh("TileMesh");
    [System.NonSerialized]
    public ModularMesh BackgroundMesh;
    [System.NonSerialized]
    public ModularMesh BuildingMesh;
    [System.NonSerialized]
    public ModularMesh StreetMesh;
    [System.NonSerialized]
    public ModularMesh OtherMesh;

    private Terrain terrain;
    private TerrainCollider tC;

    List<Street> streets = new List<Street>();

    List<StreetPolygon> streetPolygons = new List<StreetPolygon>();
    Dictionary<long, OSMWay> unusedWays = new Dictionary<long, OSMWay>();

    private Stopwatch sw = new Stopwatch();
    private System.Text.StringBuilder sB = new System.Text.StringBuilder();

    bool shouldStartProcedural = false;

    public event EventHandler ProceduralDone;
    protected void OnProceduralDone()
    {
        if (ProceduralDone != null)
            ProceduralDone(this, new EventArgs());
    }

    public void StartQuery()
    {
        Query = new OverpassQuery();
        this.Query.BoundingBox = new OSMBoundingBox
                                            (
                                                TileManager.OriginLongitude - TileManager.TileWidth / 2d + TileManager.TileWidth * TileIndex[0],
                                                TileManager.OriginLatitude - TileManager.TileWidth / 2d + TileManager.TileWidth * TileIndex[1],
                                                TileManager.OriginLongitude + TileManager.TileWidth / 2d + TileManager.TileWidth * TileIndex[0],
                                                TileManager.OriginLatitude + TileManager.TileWidth / 2d + TileManager.TileWidth * TileIndex[1]
                                            );
        this.Query.QueryDone += QueryDone;
        this.Query.DownloadOSMString();
    }
    public void QueryDone(object sender, System.EventArgs e)
    {
        this.Query.QueryDone -= QueryDone;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += EditorAppUpdate;
        sB.AppendLine("Creating Enumerator");
        enumerator = Procedural();
#else
        shouldStartProcedural = true;
#endif


    }

    IEnumerator enumerator;
#if UNITY_EDITOR
    void EditorAppUpdate()
    {
        if (!enumerator.MoveNext())
        {
            UnityEditor.EditorApplication.update -= EditorAppUpdate;
            sB.AppendLine("Procedural done");
        }
        else
        {
            //UnityEditor.SceneView.RepaintAll();
            //UnityEditor.EditorApplication.update(); //Dangerous, can freeze unity!
        }
    }
#endif
    void Update()
    {
        if (shouldStartProcedural)
        {
            StartCoroutine(Procedural());
            shouldStartProcedural = false;
        }
    }

    IEnumerator Procedural()
    {
        sB.AppendLine("Starting Procedural");
        sw.Reset();
        sw.Start();
        this.transform.position = TilePosition - new Vector3((float)TileManager.TileWidth * (float)TileManager.Scaling / 2f, 0f, (float)TileManager.TileWidth * (float)TileManager.Scaling / 2f);
        terrain = this.gameObject.GetOrAddComponent<Terrain>();
        tC = this.gameObject.GetOrAddComponent<TerrainCollider>();

        Building.Clear();
        Intersection.Clear();
        Street.Clear();
        Water.Clear();
        River.Clear();
        Bridge.Clear();
        
        unusedWays.Clear();
        streets.Clear();

        streetPolygons.Clear();

        #region LOD0
        if (lOD == 0)
        {
            Console.AddMessage("Procedural lod 0");
            #region terrain
            TerrainData terrainData = new TerrainData();

            //HeightMap
            terrainData.heightmapResolution = 257;
            float[,] heights = new float[257, 257];
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    heights[x, y] = 1f;
                }
            }

            terrainData.SetHeights(0, 0, heights);
            terrainData.size = new Vector3((float)TileManager.TileWidth * (float)TileManager.Scaling, 10f, (float)TileManager.TileWidth * (float)TileManager.Scaling);

            //SplatPrototypes
            SplatPrototype[] splatPrototypes = new SplatPrototype[1];
            splatPrototypes[0] = new SplatPrototype();
            //splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/White1px");
            splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/Terrain/Grass (Hill)");

            terrainData.splatPrototypes = splatPrototypes;

            terrain.terrainData = terrainData;
            tC.terrainData = terrainData;
            #endregion terrain
            #region mesh
            TileMesh.Clear();

            Vertex[] tileQuadVertices = new Vertex[4];
            tileQuadVertices[0] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[1] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[2] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[3] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);

            new Quad(tileQuadVertices, TileMesh, MaterialManager.GetMaterial("diffuseBrown"));

            TileMesh.FillMeshDivideMaterialsKeepMeshStructure(transform, true);
            #endregion

        }
        #endregion

        #region LOD5
        if (lOD == 5)
        {
            Console.AddMessage("Procedural lod 5");
            sw.Stop();
            sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Terrain");
            sw.Start();

            #region terrain
            if (terrain.terrainData == null)
                terrain.terrainData = new TerrainData();
            //HeightMap
            terrain.terrainData.heightmapResolution = 257;
            terrain.terrainData.alphamapResolution = 257;

            float height = 0f;

            //terrain.terrainData.SetHeights(0, 0, SRTMHeightProvider.GetInterpolatedTerrain(this.Query.BoundingBox, out height));
            terrain.terrainData.size = new Vector3((float)TileManager.TileWidth * (float)TileManager.Scaling, height, (float)TileManager.TileWidth * (float)TileManager.Scaling);

            //SplatPrototypes
            SplatPrototype[] splatPrototypes = new SplatPrototype[3];
            splatPrototypes[0] = new SplatPrototype();
            splatPrototypes[1] = new SplatPrototype();
            splatPrototypes[2] = new SplatPrototype();

            splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/Terrain/GoodDirt");
            splatPrototypes[1].texture = (Texture2D)Resources.Load("Textures/Terrain/Grass&Rock");
            splatPrototypes[2].texture = (Texture2D)Resources.Load("Textures/Terrain/Grass (Hill)"); 
            terrain.terrainData.splatPrototypes = splatPrototypes;

            


            float[, ,] splatmapData = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];

            //UnityEngine.Debug.Log("alphamapWidth: " + terrain.terrainData.alphamapWidth);
            //UnityEngine.Debug.Log("alphamapHeight: " + terrain.terrainData.alphamapHeight);
            //UnityEngine.Debug.Log("TerrainLayer.MinTerrainHeight: " + TerrainLayer.MinTerrainHeight);
            //UnityEngine.Debug.Log("TerrainLayer.MaxTerrainHeight: " + TerrainLayer.MaxTerrainHeight);

            for (int x = 0; x < terrain.terrainData.alphamapHeight; x++)
            {
                for (int y = 0; y < terrain.terrainData.alphamapWidth; y++)
                {
                    float terrainPointHeight = (terrain.terrainData.GetHeight(x, y) - TerrainLayer.MinTerrainHeight) / (TerrainLayer.MaxTerrainHeight - TerrainLayer.MinTerrainHeight);
                    Vector3 splat = new Vector3(0, 1, 0);

                    //if (x == 0)
                    //    UnityEngine.Debug.Log("terrainPointHeight " + terrainPointHeight);

                    if (terrainPointHeight > 0.5f)
                        splat = Vector3.Slerp(splat, new Vector3(0, 0, 1), (terrainPointHeight - 0.5f) * 2);
                    else
                        splat = Vector3.Slerp(new Vector3(1, 0, 0), splat, terrainPointHeight * 2);

                    splat.Normalize();
                    splatmapData[y, x, 0] = splat.x;
                    splatmapData[y, x, 1] = splat.y;
                    splatmapData[y, x, 2] = splat.z;
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
            //TODO

            tC.terrainData = terrain.terrainData;
            sw.Stop();
            sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Terrain done");
            sw.Start();
            #endregion terrain
            #region mesh
            TileMesh.Clear();

            if (BackgroundMesh != null)
                BackgroundMesh.Clear();
            else
                BackgroundMesh = new ModularMesh(TileMesh, "BackgroundMesh");

            if (BuildingMesh != null)
                BuildingMesh.Clear();
            else
                BuildingMesh = new ModularMesh(TileMesh, "BuildingMesh");

            if (StreetMesh != null)
                StreetMesh.Clear();
            else
                StreetMesh = new ModularMesh(TileMesh, "StreetMesh");

            if (OtherMesh != null)
                OtherMesh.Clear();
            else
                OtherMesh = new ModularMesh(TileMesh, "OtherMesh");

            Vertex[] tileQuadVertices = new Vertex[4];
            tileQuadVertices[0] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[1] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[2] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[3] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);

            new Quad(tileQuadVertices, BackgroundMesh, MaterialManager.GetMaterial("diffuseBlack"));


            yield return null;

            //Create Domain Objects
            ///Relations
            foreach (KeyValuePair<long, OSMRelation> kV in Query.OSM.relations)
            {
                OSMRelation relation = kV.Value;

                River.TryCreateFromOSM(relation, this);
                yield return null;
            }

            
            ///Ways
            foreach (KeyValuePair<long, OSMWay> kV in Query.OSM.ways)
            {
                OSMWay way = kV.Value;

                if (!way.FirstInBounds(Query.BoundingBox, Query.OSM))
                    continue;

                if (!Building.TryCreateFromOSM(way, this))
                    if (!Intersection.TryCreateFromOSM(way, this))
                        if (!Water.TryCreateFromOSM(way, this))
                        {}

                yield return null;
            }

            //Nodes!?
            foreach (KeyValuePair<long, OSMNode> kV in Query.OSM.nodes)
            {
                OSMNode node = kV.Value;
                TrafficSignal.TryCreateFromOSM(node, this);
            }

            //Create Streets (and Bridges)
            Street.CreateStreets(this);



            //CreateTheMeshes
            Street.CreateAllMeshes(StreetMesh);

            Bridge.CreateAllMeshes(StreetMesh);
            Intersection.CreateAllMeshes(StreetMesh);

            Street.CreateAllMeshes(StreetMesh); // A second time, cause Intersections change streetproperties

            Building.CreateAllMeshes(BuildingMesh);
            Water.CreateAllMeshes(OtherMesh);
            TrafficSignal.CreateAllMeshes(OtherMesh);

            //StreetPolygon currentStreetPolygon;
            //bool hasLeftPolygon = false;
            //bool hasRightPolygon = false;
            //for (int i = 0; i < streets.Count; i++)
            //{
            //    for (int j = 0; j < streetPolygons.Count; j++)
            //    {
            //        hasLeftPolygon = streetPolygons[j].IsLefthandPolygonOf(streets[i]);
            //        hasRightPolygon = streetPolygons[j].IsRighthandPolygonOf(streets[i]);
            //    }
            //    if (!hasLeftPolygon)
            //    {
            //        if (StreetPolygon.GetLefthandStreetPolygon(streets[i], out currentStreetPolygon))
            //            streetPolygons.Add(currentStreetPolygon);
            //    }
            //    if (!hasRightPolygon)
            //    {
            //        if (StreetPolygon.GetRighthandStreetPolygon(streets[i], out currentStreetPolygon))
            //            streetPolygons.Add(currentStreetPolygon);
            //    }

            //    hasLeftPolygon = false;
            //    hasRightPolygon = false;
            //}

            //for (int i = 0; i < streetPolygons.Count; i++)
            //{
            //    streetPolygons[i].Triangulate(StreetMesh , MaterialManager.GetMaterial("error"));
            //}

            sw.Stop();
            sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Start to fill Mesh");
            sw.Start();
            TileMesh.FillMeshDivideMaterialsKeepMeshStructure(transform, true);
            sw.Stop();
            sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Done with fill Mesh");
            sw.Start();
            #endregion

            sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Procedural done");
        }
        #endregion

        sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Starting GarbageCollection");
        System.GC.Collect();
        sB.AppendLine(sw.ElapsedMilliseconds.ToString() + "ms Done with GarbageCollection");
        UnityEngine.Debug.Log(sB.ToString());

        yield return null;
        OnProceduralDone();
        yield return true;
    }

    public bool drawOSMHandles = false;
    public bool drawOSMGizmos = true;
    public bool drawStreetPolygonGizmos = false;

    public void OnDrawGizmos()
    {
        if (LayerTool.OSMGizmoMap && !LayerTool.OSMGizmoMapOnlyOnSelected && !OSMMapTools.DrawAsHandles)//Only draw Gizmos, when no control key is pressed, otherwise draw Handles (from TileEditor.OnSceneGUI)
        {
            if (Query != null)
            {
                if (Query.OSM != null)
                { 
                      Query.OSM.OnDrawGizmos(transform.position);
                }
            } 
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (LayerTool.OSMGizmoMap && !OSMMapTools.DrawAsHandles) //Only draw Gizmos, when no control key is pressed, otherwise draw Handles (from TileEditor.OnSceneGUI)
        {
            if (Query != null)
            {
                if (Query.OSM != null)
                {
                     Query.OSM.OnDrawGizmos(transform.position); 
                }
            }
        }
    }

}

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;

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
        //Console.AddMessage("Created Tile with LOD" + tile.lOD);
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
        get
        {
            return lOD;
        }
        set
        {
            lOD = Mathf.Clamp(value, 0, 5);
        }
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
    public bool done = false;

    public event EventHandler ProceduralDone;
    protected void OnProceduralDone()
    {
        SimpleClient.StatusUpdateMessage.Stop(Job.ProceduralDone);
        done = true;
        if (ProceduralDone != null)
            ProceduralDone(this, new EventArgs());


        SimpleClient.simpleClient.SendStatusUpdateMessages();
    }

    public event EventHandler ProceduralDoneLocal;
    protected void OnProceduralDoneLocal()
    {
        done = true;
        msg.Stop(Job.ProceduralDone);
        if (ProceduralDoneLocal != null)
            ProceduralDoneLocal(this, new EventArgs());
    }

    public void StartQuery()
    {

        if (this.msg != null)
        {
            this.msg.Start(Job.StartOSMQuery);
        }
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
        if (this.msg != null)
        {
            this.msg.Stop(Job.StartOSMQuery);
            this.msg.Start(Job.StartProcedural);
        }
#if UNITY_EDITOR
        //if (EditorApplication.isPlaying)
        //{
        shouldStartProcedural = true;
        //}
        //else
        //{
        UnityEditor.EditorApplication.update += EditorAppUpdate;
        //sB.AppendLine("Creating Enumerator");
        enumerator = ProceduralLocal();
        //}
#else
                shouldStartProcedural = true;
        SimpleClient.StatusUpdateMessage.Stop(Job.StartOSMQuery);
        SimpleClient.simpleClient.SendStatusUpdateMessages();
#endif

    }

    IEnumerator enumerator;
#if UNITY_EDITOR
    void EditorAppUpdate()
    {
        if (!enumerator.MoveNext())
        {
            UnityEditor.EditorApplication.update -= EditorAppUpdate;
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
            SimpleClient.StatusUpdateMessage.Start(Job.StartProcedural);
            SimpleClient.simpleClient.SendStatusUpdateMessages();
            StartCoroutine(Procedural());
            shouldStartProcedural = false;
        }
    }

    IEnumerator Procedural()
    {
        SimpleClient.StatusUpdateMessage.Stop(Job.StartProcedural);
        SimpleClient.StatusUpdateMessage.Start(Job.ProceduralPreparation);
        SimpleClient.simpleClient.SendStatusUpdateMessages();

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

        SimpleClient.StatusUpdateMessage.Stop(Job.ProceduralPreparation);

        #region LOD0
        if (lOD == 0)
        {
            Console.AddMessage("Procedural lod 0");
            Debug.Log("Procedural lod 0");
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

            ////SplatPrototypes
            //SplatPrototype[] splatPrototypes = new SplatPrototype[1];
            //splatPrototypes[0] = new SplatPrototype();
            ////splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/White1px");
            //splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/Terrain/Grass (Hill)");

            //terrainData.splatPrototypes = splatPrototypes;

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
            Debug.Log("Procedural lod 0 - Done");
        }
        #endregion

        #region LOD5
        if (lOD == 5)
        {
            SimpleClient.StatusUpdateMessage.Start(Job.CreateTerrain);
            SimpleClient.simpleClient.SendStatusUpdateMessages();

            #region terrain
            if (terrain.terrainData == null)
                terrain.terrainData = new TerrainData();
            //HeightMap
            terrain.terrainData.heightmapResolution = 257;
            terrain.terrainData.alphamapResolution = 257;

            float height = 0f;

            terrain.terrainData.SetHeights(0, 0, SRTMHeightProvider.GetInterpolatedTerrain(this.Query.BoundingBox, out height));
            terrain.terrainData.size = new Vector3((float)TileManager.TileWidth * (float)TileManager.Scaling, height, (float)TileManager.TileWidth * (float)TileManager.Scaling);

            tC.terrainData = terrain.terrainData;

            SimpleClient.StatusUpdateMessage.Stop(Job.CreateTerrain);
            #endregion terrain
            #region mesh
            SimpleClient.StatusUpdateMessage.Start(Job.MeshPreparation);
            SimpleClient.simpleClient.SendStatusUpdateMessages();
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

            SimpleClient.StatusUpdateMessage.Stop(Job.MeshPreparation);

            SimpleClient.StatusUpdateMessage.Start(Job.TileQuad);
            SimpleClient.simpleClient.SendStatusUpdateMessages();

            Vertex[] tileQuadVertices = new Vertex[4];
            tileQuadVertices[0] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[1] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[2] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[3] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);

            new Quad(tileQuadVertices, BackgroundMesh, MaterialManager.GetMaterial("diffuseBlack"));
            SimpleClient.StatusUpdateMessage.Stop(Job.TileQuad);


            yield return null;
            SimpleClient.StatusUpdateMessage.Start(Job.River);
            SimpleClient.simpleClient.SendStatusUpdateMessages();

            //Create Domain Objects
            ///Relations
            foreach (KeyValuePair<long, OSMRelation> kV in Query.OSM.relations)
            {
                OSMRelation relation = kV.Value;

                River.TryCreateFromOSM(relation, this);
                yield return null;
            }
            SimpleClient.StatusUpdateMessage.Stop(Job.River);

            SimpleClient.StatusUpdateMessage.Start(Job.Ways);
            SimpleClient.simpleClient.SendStatusUpdateMessages();

            ///Ways
            foreach (KeyValuePair<long, OSMWay> kV in Query.OSM.ways)
            {
                OSMWay way = kV.Value;

                if (!way.FirstInBounds(Query.BoundingBox, Query.OSM))
                    continue;

                if (!Building.TryCreateFromOSM(way, this))
                    if (!Intersection.TryCreateFromOSM(way, this))
                        if (!Water.TryCreateFromOSM(way, this))
                        {
                        }

                yield return null;
            }
            SimpleClient.StatusUpdateMessage.Stop(Job.Ways);

            //Nodes!?
            //foreach (KeyValuePair<long, OSMNode> kV in Query.OSM.nodes)
            //{
            //    OSMNode node = kV.Value;
            //    TrafficSignal.TryCreateFromOSM(node, this);
            //}

            //Debug.Log("CreateStreets");
            ////Create Streets (and Bridges)
            //Street.CreateStreets(this);



            //CreateTheMeshes
            //Debug.Log("CreateAllMeshes StreetMesh");
            //Street.CreateAllMeshes(StreetMesh);

            //Debug.Log("CreateAllMeshes StreetMesh Bridge");
            //Bridge.CreateAllMeshes(StreetMesh);
            //Debug.Log("CreateAllMeshes StreetMesh Intersection");
            //Intersection.CreateAllMeshes(StreetMesh);

            //Debug.Log("CreateAllMeshes StreetMesh Street");
            //Street.CreateAllMeshes(StreetMesh); // A second time, cause Intersections change streetproperties
            SimpleClient.StatusUpdateMessage.Start(Job.CreateBuildingMesh);
            SimpleClient.simpleClient.SendStatusUpdateMessages();

            Building.CreateAllMeshes(BuildingMesh);
            SimpleClient.StatusUpdateMessage.Stop(Job.CreateBuildingMesh);
            //Debug.Log("CreateAllMeshes Water");
            //Water.CreateAllMeshes(OtherMesh);
            //Debug.Log("CreateAllMeshes TrafficSignal");
            //TrafficSignal.CreateAllMeshes(OtherMesh);

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


            SimpleClient.StatusUpdateMessage.Start(Job.FillMeshDivideMaterials);
            SimpleClient.simpleClient.SendStatusUpdateMessages();
            TileMesh.FillMeshDivideMaterialsKeepMeshStructure(transform, true);
            SimpleClient.StatusUpdateMessage.Stop(Job.FillMeshDivideMaterials);
            #endregion
        }
        #endregion

        SimpleClient.StatusUpdateMessage.Start(Job.GarbageCollection);
        SimpleClient.simpleClient.SendStatusUpdateMessages();
        System.GC.Collect();
        SimpleClient.StatusUpdateMessage.Stop(Job.GarbageCollection);

        yield return null;
        SimpleClient.StatusUpdateMessage.Start(Job.ProceduralDone);
        SimpleClient.simpleClient.SendStatusUpdateMessages();
        OnProceduralDone();


        yield return true;
    }

    int jobNumber = 0;
    StatusUpdateMessage msg;
    public void SetJobInfo(int jobCount, StatusUpdateMessage msg)
    {
        this.jobNumber = jobCount;
        this.msg = msg;
    }

    IEnumerator ProceduralLocal()
    {
        msg.Stop(Job.StartProcedural);
        msg.Start(Job.ProceduralPreparation);
        SimpleClient.jobStatus[jobNumber] = msg;

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

        msg.Stop(Job.ProceduralPreparation);

        #region LOD0
        if (lOD == 0)
        {
            Console.AddMessage("Procedural lod 0");
            Debug.Log("Procedural lod 0");
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

            ////SplatPrototypes
            //SplatPrototype[] splatPrototypes = new SplatPrototype[1];
            //splatPrototypes[0] = new SplatPrototype();
            ////splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/White1px");
            //splatPrototypes[0].texture = (Texture2D)Resources.Load("Textures/Terrain/Grass (Hill)");

            //terrainData.splatPrototypes = splatPrototypes;

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
            Debug.Log("Procedural lod 0 - Done");
        }
        #endregion

        #region LOD5
        if (lOD == 5)
        {
            msg.Start(Job.CreateTerrain);
            SimpleClient.jobStatus[jobNumber] = msg;

            #region terrain
            if (terrain.terrainData == null)
                terrain.terrainData = new TerrainData();
            //HeightMap
            terrain.terrainData.heightmapResolution = 257;
            terrain.terrainData.alphamapResolution = 257;

            float height = 0f;

            terrain.terrainData.SetHeights(0, 0, SRTMHeightProvider.GetInterpolatedTerrain(this.Query.BoundingBox, out height));
            terrain.terrainData.size = new Vector3((float)TileManager.TileWidth * (float)TileManager.Scaling, height, (float)TileManager.TileWidth * (float)TileManager.Scaling);

            tC.terrainData = terrain.terrainData;

            msg.Stop(Job.CreateTerrain);
            #endregion terrain
            #region mesh
            msg.Start(Job.MeshPreparation);
            SimpleClient.jobStatus[jobNumber] = msg;
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

            msg.Stop(Job.MeshPreparation);

            msg.Start(Job.TileQuad);
            SimpleClient.jobStatus[jobNumber] = msg;

            Vertex[] tileQuadVertices = new Vertex[4];
            tileQuadVertices[0] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[1] = new Vertex(new Vector3((float)(-TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[2] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);
            tileQuadVertices[3] = new Vertex(new Vector3((float)(TileManager.TileWidth * TileManager.Scaling / 2d), 0f, (float)(-TileManager.TileWidth * TileManager.Scaling / 2d)) + TilePosition);

            new Quad(tileQuadVertices, BackgroundMesh, MaterialManager.GetMaterial("diffuseBlack"));
            msg.Stop(Job.TileQuad);


            yield return null;
            msg.Start(Job.River);
            SimpleClient.jobStatus[jobNumber] = msg;

            //Create Domain Objects
            ///Relations
            foreach (KeyValuePair<long, OSMRelation> kV in Query.OSM.relations)
            {
                OSMRelation relation = kV.Value;

                River.TryCreateFromOSM(relation, this);
                yield return null;
            }
            msg.Stop(Job.River);

            msg.Start(Job.Ways);
            SimpleClient.jobStatus[jobNumber] = msg;

            ///Ways
            foreach (KeyValuePair<long, OSMWay> kV in Query.OSM.ways)
            {
                OSMWay way = kV.Value;

                if (!way.FirstInBounds(Query.BoundingBox, Query.OSM))
                    continue;

                if (!Building.TryCreateFromOSM(way, this))
                    if (!Intersection.TryCreateFromOSM(way, this))
                        if (!Water.TryCreateFromOSM(way, this))
                        {
                        }

                yield return null;
            }
            msg.Stop(Job.Ways);

            //Nodes!?
            //foreach (KeyValuePair<long, OSMNode> kV in Query.OSM.nodes)
            //{
            //    OSMNode node = kV.Value;
            //    TrafficSignal.TryCreateFromOSM(node, this);
            //}

            //Debug.Log("CreateStreets");
            ////Create Streets (and Bridges)
            //Street.CreateStreets(this);



            //CreateTheMeshes
            //Debug.Log("CreateAllMeshes StreetMesh");
            //Street.CreateAllMeshes(StreetMesh);

            //Debug.Log("CreateAllMeshes StreetMesh Bridge");
            //Bridge.CreateAllMeshes(StreetMesh);
            //Debug.Log("CreateAllMeshes StreetMesh Intersection");
            //Intersection.CreateAllMeshes(StreetMesh);

            //Debug.Log("CreateAllMeshes StreetMesh Street");
            //Street.CreateAllMeshes(StreetMesh); // A second time, cause Intersections change streetproperties
            msg.Start(Job.CreateBuildingMesh);
            SimpleClient.jobStatus[jobNumber] = msg;

            Building.CreateAllMeshes(BuildingMesh);
            msg.Stop(Job.CreateBuildingMesh);
            //Debug.Log("CreateAllMeshes Water");
            //Water.CreateAllMeshes(OtherMesh);
            //Debug.Log("CreateAllMeshes TrafficSignal");
            //TrafficSignal.CreateAllMeshes(OtherMesh);

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


            msg.Start(Job.FillMeshDivideMaterials);
            SimpleClient.jobStatus[jobNumber] = msg;
            TileMesh.FillMeshDivideMaterialsKeepMeshStructure(transform, true);
            msg.Stop(Job.FillMeshDivideMaterials);
            #endregion
        }
        #endregion

        msg.Start(Job.GarbageCollection);
        SimpleClient.jobStatus[jobNumber] = msg;
        System.GC.Collect();
        msg.Stop(Job.GarbageCollection);

        yield return null;
        msg.Start(Job.ProceduralDone);
        SimpleClient.jobStatus[jobNumber] = msg;
        OnProceduralDoneLocal();
        msg.Stop(Job.Worker);
        msg.Stop();

        yield return true;
    }


    public bool drawOSMHandles = false;
    public bool drawOSMGizmos = true;
    public bool drawStreetPolygonGizmos = false;


    //public void OnDrawGizmos()
    //{
    //    if (LayerTool.OSMGizmoMap && !LayerTool.OSMGizmoMapOnlyOnSelected && !OSMMapTools.DrawAsHandles)//Only draw Gizmos, when no control key is pressed, otherwise draw Handles (from TileEditor.OnSceneGUI)
    //    {
    //        if (Query != null)
    //        {
    //            if (Query.OSM != null)
    //            { 
    //                  Query.OSM.OnDrawGizmos(transform.position);
    //            }
    //        } 
    //    }
    //}

    //public void OnDrawGizmosSelected()
    //{
    //    if (LayerTool.OSMGizmoMap && !OSMMapTools.DrawAsHandles) //Only draw Gizmos, when no control key is pressed, otherwise draw Handles (from TileEditor.OnSceneGUI)
    //    {
    //        if (Query != null)
    //        {
    //            if (Query.OSM != null)
    //            {
    //                 Query.OSM.OnDrawGizmos(transform.position); 
    //            }
    //        }
    //    }
    //}

}
